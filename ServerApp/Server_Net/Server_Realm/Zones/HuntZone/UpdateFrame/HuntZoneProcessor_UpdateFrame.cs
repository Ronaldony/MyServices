using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.TaskDispatcher.Node;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 업데이트 프레임 디스패치.
        /// </summary>
        private async Task DispatchUpdateFrameAsync(WorldMsg_UpdateFrame worldMsg)
        {
            var nowTime = TimeUtil.Now;

            // 프레임 델타 타임 계산.
            _frameDeltaSec = (nowTime - _lastFrameUpdateTime).TotalSeconds;
            _lastFrameUpdateTime = nowTime;

            // 이동 처리.
            // 전투, AI 등의 처리를 위치 기반으로 처리하기 때문에 모든 처리 전에 좌표를 동기화 시킴.
            UpdateFrame_Movement(_frameDeltaSec);

            // 테스크 디스패치. 
            foreach (var nodeType in _taskNodeTypes)
            {
                await _taskDispatcher.DispatchTasksAsync(nodeType);
            }

            // Barodcast.
            UpdateFrame_Braodcast();

            // Collect dead units.
            UpdateFrame_CollectDeadUnits();

            // Benchmark.
            ++_benchFrameCount;
            _benchFrameSum += (TimeUtil.Now - nowTime).TotalMilliseconds;

            if (_benchFrameCount >= 300)
            {
                _logger.LogDebug($"frame update avg: {Math.Round(_benchFrameSum / _benchFrameCount, 2)} ms, total: {Math.Round(_benchFrameSum, 2)} ms, count: {_benchFrameCount}");

                _benchFrameSum = 0;
                _benchFrameCount = 0;
            }
        }

        /// <summary>
        /// Update frame.
        /// </summary>
        private (TaskNode_WorldSection, Type_TaskNode) UpdateFrame((TaskNode_WorldSection taskNode, Type_TaskNode taskNodeType) tuple)
        {
            var taskNode = tuple.taskNode;

            switch (tuple.taskNodeType)
            {
                case Type_TaskNode.AI:
                    UpdateFrame_AI(_frameDeltaSec, taskNode.TaskSections);
                    break;
                case Type_TaskNode.Combat:
                    UpdateFrame_Combat(_frameDeltaSec, taskNode.TaskSections);
                    break;
                default:
                    break;
            }

            return tuple;
        }

        /// <summary>
        /// 업데이트 프레임 적용.
        /// </summary>
        private void UpdateFrame_Braodcast()
        {
            Parallel.ForEach(_worldGroup.WorldSectionDict.Values, ws =>
            {
                // 섹션에 새로 진입한 유저 리스트.
                var newEnterPlayers = new List<PlayerUnit>();

                // Update 게임 패킷.
                var updateSection = _memoryPoolService.Rent<GamePacketSC_UpdateSection>();
                var updateUnitMovement = _memoryPoolService.Rent<GamePacketSC_UpdateUnitMovement>();
                var updateUnitStat = _memoryPoolService.Rent<GamePacketSC_UpdateUnitStat>();

                // 플레이어 업데이트.
                foreach (var pu in ws.GetPlayerUnits())
                {
                    // 섹션 업데이트.
                    if (pu.MovementHandler.IsSectionUpdated())
                    {
                        if (false == updateSection.PlayInfos.TryAdd(pu.UnitId, pu.PlayInfo)) _logger.LogError($"{ws.Section} failed to add new player section. unitId: {pu.UnitId}");

                        // new players.
                        newEnterPlayers.Add(pu);
                    }

                    // 스텟 변화.
                    if (pu.StatHandler.IsStatUpdated())
                    {
                        if (false == updateUnitStat.UnitStats.TryAdd(pu.UnitId, pu.PlayInfo.Stat)) _logger.LogError($"{ws.Section} failed to add player stat. unitId: {pu.UnitId}");
                    }
                }

                // 몹 업데이트.
                foreach (var mu in ws.GetMobUnits())
                {
                    // 섹션 업데이트.
                    if (mu.MovementHandler.IsSectionUpdated())
                    {
                        if (false == updateSection.Mobs.TryAdd(mu.UnitId, mu.Mob)) _logger.LogError($"{ws.Section} failed to add new mob section. unitId: {mu.UnitId}");
                    }

                    // 이동 상태 변화.
                    if (mu.MovementHandler.IsMoveStatusUpdated())
                    {
                        if (false == updateUnitMovement.UnitLocations.TryAdd(mu.UnitId, mu.Mob.Location)) _logger.LogError($"{ws.Section} failed to add mob movment. unitId: {mu.UnitId}");
                    }

                    // 스텟 변화.
                    if (mu.StatHandler.IsStatUpdated())
                    {
                        if (false == updateUnitStat.UnitStats.TryAdd(mu.UnitId, mu.Mob.Stat)) _logger.LogError($"{ws.Section} failed to add mob stat. unitId: {mu.UnitId}");
                    }
                }

                // 패킷 전송.
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateSection, updateSection);
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateUnitMovement, updateUnitMovement);
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateUnitStat, updateUnitStat);

                // Release.
                _memoryPoolService.Release(updateSection);
                _memoryPoolService.Release(updateUnitMovement);
                _memoryPoolService.Release(updateUnitStat);

                // 섹션에 새로 진입한 유저 검사.
                // TODO: 진입 전 섹션과 현재 섹션을 비교하여 갱신되지 않은 섹션 정보만 전달해줄 수 없을까? => 각 플레이어마다 다르기 떄문에 필터에서 부하 발생.
                if (newEnterPlayers.Any())
                {
                    // 섹션에 새로 진입한 유저에게 해당 섹션 주변 정보 전달하기.
                    var updateSectionNewEnter = _memoryPoolService.Rent<GamePacketSC_UpdateSectionNewEnter>();

                    updateSectionNewEnter.PlayInfos = ws.DetectAroundPlayers().ToDictionary(d => d.UnitId, d => d.PlayInfo);
                    updateSectionNewEnter.Mobs = ws.DetectAroundMobs().ToDictionary(d => d.UnitId, d => d.Mob);

                    foreach (var pu in newEnterPlayers)
                    {
                        SendPacket(pu.Pid, GamePacketCode.SC_UpdateSectionNewEnter, updateSectionNewEnter);
                    }

                    _memoryPoolService.Release(updateSectionNewEnter);
                }
            });
        }

        /// <summary>
        /// Collect dead units.
        /// </summary>
        private void UpdateFrame_CollectDeadUnits()
        {
            /////////////////////////////////////////////////////
            // 죽은 몹 수거.
            foreach (var unit in _worldUnitCollector.DeadUnits.Values)
            {
                // Leave section.
                switch (unit.UnitType)
                {
                    case Type_Unit.Player:
                        var pu = unit as PlayerUnit;

                        unit.WorldSection.LeavePlayer(unit.UnitId);
                        _worldGroup.PlayerUnitDict.Remove(pu.Pid);
                        break;
                    case Type_Unit.Mob:
                        unit.WorldSection.LeaveMob(unit.UnitId);
                        _worldGroup.MobUnitDict.Remove(unit.UnitId);
                        break;
                }

                // Return unit.
                _unitFactory.Return(unit);
            }

            _worldUnitCollector.DeadUnits.Clear();
        }
    }
}
