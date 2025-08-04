using GameCore.GamePacket;

namespace Server_Realm.Zones
{
    /// <summary>
    /// IZoneProcessor.
    /// </summary>
    public interface IZoneProcessor
    {
        /// <summary>
        /// Tell.
        /// </summary>
        void Tell<T>(T msg);

        /// <summary>
        /// Send pakcet.
        /// </summary>
        public bool SendPacket<T>(string pid, int packetCode, T packet) where T : GamePacketBase_SC;

        /// <summary>
        /// Send pakcet bytes.
        /// </summary>
        public bool SendPacket<T>(string pid, int packetCode, byte[] packetBytes) where T : GamePacketBase_SC;
    }
}
