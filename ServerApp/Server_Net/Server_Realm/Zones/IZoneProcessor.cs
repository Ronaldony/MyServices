namespace Server_Realm.Zones
{
    using GameCore.GamePacket;
    using Server_Realm.Entities;

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
        /// Get player unit.
        /// </summary>
        public PlayerUnit GetPlayer(string pid);

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
