
namespace Server_Realm.Zones.HuntZone
{
    using GameCore.GamePacket;
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Send packet.
        /// </summary>
        private bool SendPacket<T>(string pid, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _huntZoneServer.Send(pid, packetCode, packetBytes);
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(string ignorePid, SectionId section, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var targetSection in GetAroundSections(section))
            {
                foreach (var pu in _worldGroup.WorldSectionDict[targetSection].GetPlayUnits())
                {
                    if (!ignorePid.Equals(pu.Pid))
                    {
                        _huntZoneServer.Send(pu.Pid, packetCode, packetBytes);
                    }
                }
            }
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        private void SendPacket_AroundSections<T>(SectionId section, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 주변 섹션에 전파.
            foreach (var targetSection in GetAroundSections(section))
            {
                foreach (var pu in _worldGroup.WorldSectionDict[targetSection].GetPlayUnits())
                {
                    _huntZoneServer.Send(pu.Pid, packetCode, packetBytes);
                }
            }
        }
    }
}
