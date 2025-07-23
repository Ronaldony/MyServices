using Microsoft.Extensions.Logging;

namespace Server_World.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_World.Core.GameEvents;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Process MoveSectionEvent.
        /// </summary>
        public void Proc_MoveSectionEvent(MoveSectionEvent moveSectionEvent)
        {
            // Rent.
            var aroundFromSections = _router.GetAroundSections(moveSectionEvent.RouteSection);
            var sectionMovePacket = _memoryPoolService.Rent<GamePacketSC_UpdateSectionMove>();

            foreach (var section in _worldGroup.ManagedSections)
            {
                // 출처 섹션의 관리 섹션인지 확인.
                if (aroundFromSections.Contains(section))
                {
                    // 유닛 추가.
                    sectionMovePacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Select(d => d.Value.PlayInfo));
                    sectionMovePacket.Enemies.AddRange(_worldGroup.SectionEnemyDict[section].Select(d => d.Value.Enemy));
                }
            }

            // Send packet.
            if (sectionMovePacket.Enemies.Any() || sectionMovePacket.PlayInfos.Any())
            {
                SendPacket(moveSectionEvent.MovePids, GamePacketCode.SC_UpdateSectionMove, sectionMovePacket);
            }

            _memoryPoolService.Release(sectionMovePacket);
        }
    }
}
