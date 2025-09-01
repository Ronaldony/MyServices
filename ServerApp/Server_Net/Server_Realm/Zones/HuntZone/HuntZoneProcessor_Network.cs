
namespace Server_Realm.Zones.HuntZone
{
    using GameCore.GamePacket;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Send packet.
        /// </summary>
        public bool SendPacket<T>(string pid, int packetCode, T packet)
            where T : GamePacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _zoneServer.Send(pid, packetCode, packetBytes);
        }

        /// <summary>
        /// Send pakcet bytes.
        /// </summary>
        public bool SendPacket<T>(string pid, int packetCode, byte[] packetBytes)
            where T : GamePacketBase_SC
        {
            // Send.
            return _zoneServer.Send(pid, packetCode, packetBytes);
        }
    }
}
