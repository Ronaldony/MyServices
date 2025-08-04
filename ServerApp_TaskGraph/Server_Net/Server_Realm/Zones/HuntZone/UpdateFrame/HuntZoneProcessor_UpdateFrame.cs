using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;
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
            UpdateFrame_AI(_frameDeltaSec);

            // 테스크 디스패처 실행. 
            foreach (var nodeType in _taskNodeTypes)
            {
                await _taskDispatcher.DispatchTasksAsync(nodeType);
            }

            // Barodcast.
            UpdateFrame_Braodcast();

            // Collect dead units.
            UpdateFrame_CollectDeadUnits();

            _logger.LogDebug($"frame update time: {(TimeUtil.Now - nowTime).Milliseconds}");
        }

        /// <summary>
        /// Update frame.
        /// </summary>
        private (TaskNode_WorldSection, Type_TaskNode) UpdateFrame((TaskNode_WorldSection taskNode, Type_TaskNode taskNodeType) tuple)
        {
            var taskNode = tuple.taskNode;

            switch (tuple.taskNodeType)
            {
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
                var updateUnitMovement = _memoryPoolService.Rent<GamePacketSC_UpdateUnitMovement>();
                var updateUnitStat = _memoryPoolService.Rent<GamePacketSC_UpdateUnitStat>();

                // 플레이어 업데이트.
                foreach (var pu in ws.GetPlayerUnits())
                {
                    // 이동.
                    if (pu.MovementHandler.NeedsStatusUpdated())
                    {
                        updateUnitMovement.UnitLocations.Add(pu.UnitId, pu.PlayInfo.Location);
                    }

                    // 스텟.
                    if (pu.StatHandler.NeedsStatusUpdated())
                    {
                        updateUnitStat.UnitStats.Add(pu.UnitId, pu.PlayInfo.Stat);
                    }
                }

                // 몹 업데이트.
                foreach (var mu in ws.GetMobUnits())
                {
                    // 이동.
                    if (mu.MovementHandler.NeedsStatusUpdated())
                    {
                        updateUnitMovement.UnitLocations.Add(mu.UnitId, mu.Mob.Location);
                    }

                    // 스텟.
                    if (mu.StatHandler.NeedsStatusUpdated())
                    {
                        updateUnitStat.UnitStats.Add(mu.UnitId, mu.Mob.Stat);
                    }
                }

                // 패킷 전송.
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateUnitMovement, updateUnitMovement);
                ws.SendPacket_AroundSections(GamePacketCode.SC_UpdateUnitStat, updateUnitStat);

                _memoryPoolService.Release(updateUnitMovement);
                _memoryPoolService.Release(updateUnitStat);
            });
        }

        /// <summary>
        /// Collect dead units.
        /// </summary>
        private void UpdateFrame_CollectDeadUnits()
        {
            // Collect dead units.
        }
    }
}
