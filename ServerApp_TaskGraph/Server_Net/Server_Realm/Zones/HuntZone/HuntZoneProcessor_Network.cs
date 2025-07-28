
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
        private bool SendPacket<T>(long unitId, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _huntZoneServer.Send(unitId, packetCode, packetBytes);
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
                foreach (var pu in _worldGroup.WorldSectionDict[targetSection].GetPlayerUnits())
                {
                    if (!ignorePid.Equals(pu.Pid))
                    {
                        _huntZoneServer.Send(pu.UnitId, packetCode, packetBytes);
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
                foreach (var pu in _worldGroup.WorldSectionDict[targetSection].GetPlayerUnits())
                {
                    _huntZoneServer.Send(pu.UnitId, packetCode, packetBytes);
                }
            }
        }
    }
}
