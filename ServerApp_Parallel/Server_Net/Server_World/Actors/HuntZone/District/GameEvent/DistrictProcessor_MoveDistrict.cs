namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas.GameEvents;

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
            var netPacket = _memoryPoolService.Rent<NetPacketSC_UpdateSection>();

            // 현재 구역의 기존 플레이어에게 새로 진입한 플레이어 전송.
            if (moveDistrictEvent.WorldEnemies.Any() || moveDistrictEvent.WorldPlayers.Any())
            {
                netPacket.PlayInfos.AddRange(moveDistrictEvent.WorldPlayers.Select(d => d.PlayInfo));
                netPacket.Enemies.AddRange(moveDistrictEvent.WorldEnemies.Select(d => d.Enemy));

                // 패킷 전송.
                SendPacket_ManagedSections(NetPacketCode.SC_UpdateSection, netPacket);

                netPacket.PlayInfos.Clear();
                netPacket.Enemies.Clear();
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
                        netPacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Values.Select(d => d.PlayInfo));
                        netPacket.Enemies.AddRange(_worldGroup.SectionEnemyDict[section].Values.Select(d => d.Enemy));
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
                        netPacket.PlayInfos.AddRange(_worldGroup.SectionPlayerDict[section].Values.Select(d => d.PlayInfo));
                        netPacket.Enemies.AddRange(_worldGroup.SectionEnemyDict[section].Values.Select(d => d.Enemy));
                    }
                }
            }

            // 현재 구역에 새로 진입한 플레이어에게 주변 섹션 전송.
            if (netPacket.PlayInfos.Any() || netPacket.Enemies.Any())
            {
                SendPacket(moveDistrictEvent.WorldPlayers.Select(d => d.GameSession).ToList(), NetPacketCode.SC_UpdateSection, netPacket);
            }

            // Release.
            _memoryPoolService.Release(netPacket);
        }
    }
}
