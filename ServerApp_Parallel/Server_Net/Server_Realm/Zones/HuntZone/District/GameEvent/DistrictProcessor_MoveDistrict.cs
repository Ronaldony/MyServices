using Dapper;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.GameEvents;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Process MoveDistrictEvent.
        /// </summary>
        public void Proc_MoveDistrictEvent(MoveDistrictEvent moveDistrictEvent)
        {
            var gamePacket = _memoryPoolService.Rent<GamePacketSC_UpdateSectionMove>();

            // 현재 구역의 기존 플레이어에게 새로 진입한 플레이어 전송.
            if (moveDistrictEvent.EnemyUnits.Any() || moveDistrictEvent.PlayerUnits.Any())
            {
                gamePacket.PlayInfos.AddRange(moveDistrictEvent.PlayerUnits.Select(d => d.PlayInfo));
                gamePacket.Enemies.AddRange(moveDistrictEvent.EnemyUnits.Select(d => d.Mob));

                // 패킷 전송.
                SendPacket_ManagedSections(GamePacketCode.SC_UpdateSectionMove, gamePacket);

                gamePacket.Dispose();
            }

            // 관리 섹션인지 검사.
            if (IsManagedSection(moveDistrictEvent.RouteSection))
            {
                // 현재 구역으로 이동한 경우, 구역에 플레이어 추가.
                foreach (var pu in moveDistrictEvent.PlayerUnits)
                {
                    SpawnPlayerIntoZone(pu);
                }

                // 새로운 시야 전송.
                foreach (var section in _worldGroup.ManagedSections)
                {
                    if (section != moveDistrictEvent.RouteSection)
                    {
                        gamePacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Values.Select(d => d.PlayInfo));
                        gamePacket.Enemies.AddRange(_worldGroup.SectionMobDict[section].Values.Select(d => d.Mob));
                    }
                }
            }
            else
            {
                // 현재 구역이 주변 구역인 경우, 새로운 시야 전송.
                var aroundFromSections = _router.GetAroundSections(moveDistrictEvent.RouteSection);

                foreach (var section in _worldGroup.ManagedSections)
                {
                    // 주변 섹션인지 검사.
                    if (aroundFromSections.Contains(section))
                    {
                        gamePacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Values.Select(d => d.PlayInfo));
                        gamePacket.Enemies.AddRange(_worldGroup.SectionMobDict[section].Values.Select(d => d.Mob));
                    }
                }
            }

            // 현재 구역에 새로 진입한 플레이어에게 주변 섹션 전송.
            if (gamePacket.PlayInfos.Any() || gamePacket.Enemies.Any())
            {
                SendPacket(moveDistrictEvent.PlayerUnits.Select(d => d.Pid).ToList(), GamePacketCode.SC_UpdateSectionMove, gamePacket);
            }

            // Release.
            _memoryPoolService.Release(gamePacket);
        }
    }
}
