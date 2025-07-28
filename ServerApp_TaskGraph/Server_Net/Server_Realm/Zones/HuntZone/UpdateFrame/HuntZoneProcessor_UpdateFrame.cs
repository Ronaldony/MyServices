using ServerEngine.Core.Util;
using System.Collections.Concurrent;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
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

            // 테스크 디스패처 실행.
            await _taskDispatcher.DispatchAndExecuteTasks();
        }

        /// <summary>
        /// Update frame.
        /// </summary>
        private void UpdateFrame(TaskNode_WorldSection taskNode)
        {
            foreach (var srcSection in taskNode.TaskSections)
            {
                // 유닛 업데이트.
                var worldSection = _worldGroup.WorldSectionDict[taskNode.Section];

                // 이동 업데이트.
                UpdateFrame_Movement(_frameDeltaSec, worldSection);

                // 유닛 충돌 처리.
                UpdateFrame_Combat(_frameDeltaSec, worldSection);

                // 업데이트 프레임 적용.
                UpdateFrame_Broadcast();

                // 작업이 완료된 후 본 노드에 의지하는 노드 작업을 전달.
                if (taskNode.DependentNode != null)
                {
                    taskNode.DependentNode.NodeGroup.ExecuteNextTaskNode(taskNode.DependentNode);
                }
            }
        }

        /// <summary>
        /// 업데이트 프레임 적용.
        /// </summary>
        private void UpdateFrame_Broadcast()
        {
            //// Apply - move section.
            //UpdateFrame_ApplyMoveSection(updateFrame.MoveSectionUnits);

            //// Apply - hit unit.
            //UpdateFrame_ApplyHitUnit(updateFrame.HitUnits);

            
        }

        /// <summary>
        /// 이동 정보 전파.
        /// </summary>
        private void UpdatFrame_BroadcastMovement()
        {
        }

        /// <summary>
        /// 이동 정보 전파.
        /// </summary>
        private void UpdateFrame_BroadcastStat()
        {
        }

        /// <summary>
        /// 장비 정보 전파.
        /// </summary>
        private void UpdateFrame_BraodcastEquipment()
        {

        }
    }
}
