namespace Server_World.Actors.HuntZone.District
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
        /// Process MoveDistrictEvent.
        /// </summary>
        public void Proc_MoveDistrictEvent(MoveDistrictEvent moveDistrictEvent)
        {
            var GamePacket = _memoryPoolService.Rent<GamePacketSC_UpdateSection>();

            // 현재 구역의 기존 플레이어에게 새로 진입한 플레이어 전송.
            if (moveDistrictEvent.WorldEnemies.Any() || moveDistrictEvent.WorldPlayers.Any())
            {
                GamePacket.PlayInfos.AddRange(moveDistrictEvent.WorldPlayers.Select(d => d.PlayInfo));
                GamePacket.Enemies.AddRange(moveDistrictEvent.WorldEnemies.Select(d => d.Enemy));

                // 패킷 전송.
                SendPacket_ManagedSections(GamePacketCode.SC_UpdateSection, GamePacket);

                GamePacket.PlayInfos.Clear();
                GamePacket.Enemies.Clear();
            }

            // 관리 섹션인지 검사.
            if (IsManagedSection(moveDistrictEvent.RouteSection))
            {
                // 현재 구역으로 이동한 경우.
                // 구역에 플레이어 추가.
                foreach (var wp in moveDistrictEvent.WorldPlayers)
                {
                    AddPlayerIntoDistrict(wp);
                }

                // 새로운 시야 전송.
                foreach (var section in _worldGroup.ManagedSections)
                {
                    if (section != moveDistrictEvent.RouteSection)
                    {
                        GamePacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Values.Select(d => d.PlayInfo));
                        GamePacket.Enemies.AddRange(_worldGroup.SectionEnemyDict[section].Values.Select(d => d.Enemy));
                    }
                }
            }
            else
            {
                // 현재 구역이 주변 구역인 경우.
                // 새로운 시야 전송.
                var aroundFromSections = _router.GetAroundSections(moveDistrictEvent.RouteSection);

                foreach (var section in _worldGroup.ManagedSections)
                {
                    // 주변 섹션인지 검사.
                    if (aroundFromSections.Contains(section))
                    {
                        GamePacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Values.Select(d => d.PlayInfo));
                        GamePacket.Enemies.AddRange(_worldGroup.SectionEnemyDict[section].Values.Select(d => d.Enemy));
                    }
                }
            }

            // 현재 구역에 새로 진입한 플레이어에게 주변 섹션 전송.
            if (GamePacket.PlayInfos.Any() || GamePacket.Enemies.Any())
            {
                SendPacket(moveDistrictEvent.WorldPlayers.Select(d => d.GameSession).ToList(), GamePacketCode.SC_UpdateSection, GamePacket);
            }

            // Release.
            _memoryPoolService.Release(GamePacket);
        }
    }
}
