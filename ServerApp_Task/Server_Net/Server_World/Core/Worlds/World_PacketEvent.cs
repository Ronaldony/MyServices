using Server_World.Core.Datas.GameEvents;

namespace Server_World.Core.Worlds
{
    /// <summary>
    /// World_PacketEvent.
    /// </summary>
    public class World_PacketEvent
    {
        /// <summary>
        /// World player.
        /// </summary>
        public World_Player Player { get; set; }

        /// <summary>
        /// World NetPacket.
        /// </summary>
        public NetPacketEvent NetPacket { get; set; }
    }
}
