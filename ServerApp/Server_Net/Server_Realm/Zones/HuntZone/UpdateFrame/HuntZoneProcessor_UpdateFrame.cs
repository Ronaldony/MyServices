using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_Realm.Zones.HuntZone
{
    using Couchbase.Management.Views;
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
            // 프레임 시간차.
            var nowTime = TimeUtil.Now;

            // update frame times.
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

            //_logger.LogDebug($"frame update time: {(TimeUtil.Now - nowTime).Milliseconds}");
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
                var newEnterPlayers = new List<PlayerUnit>();

                var updateSectionOld = _memoryPoolService.Rent<GamePacketSC_UpdateSection>();
                var updateUnitMovement = _memoryPoolService.Rent<GamePacketSC_UpdateUnitMovement>();
                var updateUnitStat = _memoryPoolService.Rent<GamePacketSC_UpdateUnitStat>();

                // 플레이어 업데이트.
                foreach (var pu in ws.GetPlayerUnits())
                {
                    // 섹션 업데이트.
                    if (pu.MovementHandler.IsSectionUpdated())
                    {
                        // Old player.
                        if (false == updateSectionOld.PlayInfos.TryAdd(pu.UnitId, pu.PlayInfo)) _logger.LogDebug($"{ws.Section} add new player section. unitId: {pu.UnitId}");
                        else _logger.LogError($"{ws.Section} failed to add new player section. unitId: {pu.UnitId}");

                        // new players.
                        newEnterPlayers.Add(pu);
                    }

                    // 스텟 변화.
                    if (pu.StatHandler.IsStatUpdated())
                    {
                        if (updateUnitStat.UnitStats.TryAdd(pu.UnitId, pu.PlayInfo.Stat)) _logger.LogDebug($"section {ws.Section} add player stat. unitId: {pu.UnitId}");
                        else _logger.LogError($"section {ws.Section} failed to add player stat. unitId: {pu.UnitId}");
                    }
                }

                // 몹 업데이트.
                foreach (var mu in ws.GetMobUnits())
                {
                    // 이동 상태 변화.
                    if (mu.MovementHandler.IsMoveStatusUpdated())
                    {
                        if (updateUnitMovement.UnitLocations.TryAdd(mu.UnitId, mu.Mob.Location))
                        {
                            _logger.LogDebug($"section {ws.Section} add mob movment. movment: {mu.UnitId}");
                        }
                        else
                        {
                            _logger.LogError($"section {ws.Section} failed to add mob movment. unitId: {mu.UnitId}");
                        }
                    }

                    // 이동 상태 변화.
                    if (mu.MovementHandler.IsMoveStatusUpdated())
                    {
                        if (updateUnitMovement.UnitLocations.TryAdd(mu.UnitId, mu.Mob.Location))
                        {
                            _logger.LogDebug($"section {ws.Section} add mob movment. movment: {mu.UnitId}");
                        }
                        else
                        {
                            _logger.LogError($"section {ws.Section} failed to add mob movment. unitId: {mu.UnitId}");
                        }
                    }

                    // 스텟 변화.
                    if (mu.StatHandler.IsStatUpdated())
                    {
                        if (updateUnitStat.UnitStats.TryAdd(mu.UnitId, mu.Mob.Stat))
                        {
                            _logger.LogDebug($"section {ws.Section} add mob stat. movment: {mu.UnitId}");
                        }
                        else
                        {
                            _logger.LogError($"section {ws.Section} failed to add mob stat. unitId: {mu.UnitId}");
                        }
                    }
                }

                // 패킷 전송.
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateUnitMovement, updateUnitMovement);
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateUnitStat, updateUnitStat);

                _memoryPoolService.Release(updateSectionOld);
                _memoryPoolService.Release(updateUnitMovement);
                _memoryPoolService.Release(updateUnitStat);

                // Update section to new enter players.
                if (newEnterPlayers.Any())
                {
                    var updateSection = _memoryPoolService.Rent<GamePacketSC_UpdateSection>();

                    var aroundSectionPlayers = ws.DetectAroundPlayers();
                    var aroundSectionMobs = ws.DetectAroundMobs();

                    foreach (var pu in newEnterPlayers)
                    {
                        SendPacket(pu.Pid, GamePacketCode.SC_UpdateSection, updateSection);
                    }

                    _memoryPoolService.Release(updateSection);
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
            foreach (var mu in _worldUnitCollector.DeadMobUnitDict.Values)
            {
                // Leave section.
                mu.WorldSection.LeaveMob(mu.UnitId);

                // Leave zone.
                _worldGroup.MobUnitDict.Remove(mu.UnitId);

                // Return unit.
                _unitFactory.Return(mu);
            }

            _worldUnitCollector.DeadMobUnitDict.Clear();
        }
    }
}
