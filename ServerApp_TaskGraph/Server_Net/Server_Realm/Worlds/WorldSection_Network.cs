
namespace Server_Realm.Worlds
{
    using GameCore.GamePacket;

    /// <summary>
    /// WorldSection.
    /// </summary>
    public partial class WorldSection
    {
        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        public void SendPacket_AroundSections<T>(int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 현재 섹션에 전파.
            foreach (var pu in GetPlayerUnits())
            {
                _zoneProcessor.SendPacket<T>(pu.Pid, packetCode, packetBytes);
            }

            // 주변 섹션에 전파.
            foreach (var ws in _aroundWorldSections)
            {
                foreach (var pu in ws.GetPlayerUnits())
                {
                    _zoneProcessor.SendPacket<T>(pu.Pid, packetCode, packetBytes);
                }
            }
        }

        /// <summary>
        /// Send packet for around sections.
        /// </summary>
        public void SendPacket_AroundSections<T>(long ignoreUnitId, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize<T>(packet);

            // 현재 섹션에 전파.
            foreach (var pu in GetPlayerUnits())
            {
                // Ignore unit id.
                if (pu.UnitId == ignoreUnitId)
                {
                    continue;
                }

                _zoneProcessor.SendPacket<T>(pu.Pid, packetCode, packetBytes);
            }

            // 주변 섹션에 전파.
            foreach (var ws in _aroundWorldSections)
            {
                foreach (var pu in ws.GetPlayerUnits())
                {
                    _zoneProcessor.SendPacket<T>(pu.Pid, packetCode, packetBytes);
                }
            }
        }
    }
}
