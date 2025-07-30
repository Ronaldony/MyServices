using ServerEngine.Core.Util;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;
    using Server_Realm.TaskDispatcher.Node;
    using Server_Realm.Worlds;
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
            await UpdateFrame_MovementAsync(_frameDeltaSec);

            // 테스크 디스패처 실행.
            foreach (var nodeType in _taskNodeTypes)
            {
                await _taskDispatcher.DispatchAndExecuteTasks(nodeType);
            }

            await UpdateFrame_AIAsync(_frameDeltaSec);
        }

        /// <summary>
        /// Update frame.
        /// </summary>
        private void UpdateFrame((TaskNode_WorldSection taskNode, Type_TaskNode taskNodeType) tuple)
        {
            var taskNode = tuple.taskNode;
            var taskNodeType = tuple.taskNodeType;

            switch (taskNodeType)
            {
                case Type_TaskNode.Combat:
                    UpdateFrame_Combat(_frameDeltaSec, taskNode.TaskSections);
                    break;
                default:
                    break;
            }

            // 작업이 완료된 후 본 노드에 의지하는 노드 작업을 전달.
            taskNode.DependentNode?.NodeGroup.ExecuteNextTaskNode(taskNode.DependentNode, taskNodeType);

            if (taskNode.NextNode != null)
            {
                // 다음 노드의 의존 노드가 없는 경우, 해당 노드 작업 스케줄링.
                if (taskNode.NextNode.DependancyNode == null)
                {
                    taskNode.NodeGroup.ExecuteNextTaskNode(taskNode.NextNode, taskNodeType);
                }
            }
            else
            {
                taskNode.NodeGroup.AlertCompletedToObserver();
            }
        }

        /// <summary>
        /// 업데이트 프레임 적용.
        /// </summary>
        private void UpdateFrame_Broadcast(World_Section worldSection)
        {
            var updateUnitMovement = _memoryPoolService.Rent<GamePacketSC_UpdateUnitMovement>();
            var updateUnitStat = _memoryPoolService.Rent<GamePacketSC_UpdateUnitStat>();

            // 플레이어.
            foreach (var pu in worldSection.GetPlayerUnits())
            {
                // 이동.
                if (pu.MovementHandler.RefreshStatus())
                {
                    updateUnitMovement.UnitLocationDict.Add(pu.UnitId, pu.PlayInfo.Location);
                }

                // 스텟.
                if (pu.StatHandler.RefreshStatus())
                {
                    updateUnitStat.UnitStatDict.Add(pu.UnitId, pu.PlayInfo.Stat);
                }
            }

            foreach (var mu in worldSection.GetMobUnits())
            {
                // 이동.
                if (mu.MovementHandler.RefreshStatus())
                {
                    updateUnitMovement.UnitLocationDict.Add(mu.UnitId, mu.Mob.Location);
                }

                // 스텟.
                if (mu.StatHandler.RefreshStatus())
                {
                    updateUnitStat.UnitStatDict.Add(mu.UnitId, mu.Mob.Stat);
                }
            }

            // 패킷 전송.
            SendPacket_AroundSections(worldSection.Section, GamePacketCode.SC_UpdateUnitMovement, updateUnitMovement);
            SendPacket_AroundSections(worldSection.Section, GamePacketCode.SC_UpdateUnitStat, updateUnitStat);

            // Release.
            _memoryPoolService.Release(updateUnitMovement);
            _memoryPoolService.Release(updateUnitStat);
        }
    }
}
