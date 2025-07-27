using ServerEngine.Core.Util;
using System.Collections.Concurrent;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Worlds.Update;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트.
        /// </summary>
        private void UpdateFrame(WorldMsg_UpdateFrame worldMsg)
        {
            // 프레임 시간차.
            var nowTime = TimeUtil.Now;
            var frameDeltaTime = (nowTime - _lastFrameUpdateTime).TotalSeconds;

            // refersh frame time.
            _lastFrameUpdateTime = nowTime;

            // TaskSection 스케줄링.
            var taskNodes = _taskDispatcher.GetTaskNodes();

            var updateFrame = _memoryPoolService.Rent<WorldUpdate_Frame>();

            // 유닛 업데이트.
            UpdateFrame_UpdateUnits(frameDeltaTime, updateFrame.MoveSectionUnits);

            // 유닛 충돌 처리.

            // 업데이트 프레임 적용.
            UpdateFrame_Apply(updateFrame);

            // Release memory.
            _memoryPoolService.Release(updateFrame.MoveSectionUnits.Values);
            _memoryPoolService.Release(updateFrame.HitUnits.Values);
            _memoryPoolService.Release(updateFrame);
        }

        /// <summary>
        /// 업데이트 프레임 적용.
        /// </summary>
        private void UpdateFrame_Apply(WorldUpdate_Frame updateFrame)
        {
            // Apply - move section.
            UpdateFrame_ApplyMoveSection(updateFrame.MoveSectionUnits);

            // Apply - hit unit.
            UpdateFrame_ApplyHitUnit(updateFrame.HitUnits);
        }

        /// <summary>
        /// 업데이트 프레임 적용 - 섹션 이동.
        /// </summary>
        private void UpdateFrame_ApplyMoveSection(ConcurrentDictionary<SectionId, WorldUpdate_SectionUnit> moveSectionUnit)
        {
            // Rent.
            var updateSectionPacket = _memoryPoolService.Rent<GamePacketSC_UpdateSectionMove>();

            // 섹션 이동 유닛 처리.
            foreach (var pair in moveSectionUnit)
            {
                var sectionUnit = pair.Value;

                if (false == sectionUnit.PlayInfoDict.Any() 
                    && false == sectionUnit.MobDict.Any())
                {
                    continue;
                }

                // 섹션 이동 유닛.
                updateSectionPacket.PlayInfos.AddRange(sectionUnit.PlayInfoDict.Values);
                updateSectionPacket.Enemies.AddRange(sectionUnit.MobDict);

                // Send packet.
                SendPacket_AroundSections(pair.Key, GamePacketCode.SC_UpdateSectionMove, updateSectionPacket);

                // clear.
                updateSectionPacket.PlayInfos.Clear();
                updateSectionPacket.Enemies.Clear();
            }

            // Release.
            _memoryPoolService.Release<GamePacketSC_UpdateSectionMove>(updateSectionPacket);
        }

        /// <summary>
        /// 업데이트 프레임 적용 - 피격 유닛.
        /// </summary>
        private void UpdateFrame_ApplyHitUnit(ConcurrentDictionary<SectionId, WorldUpdate_HitUnit> hitSectionUnit)
        {

        }
    }
}
