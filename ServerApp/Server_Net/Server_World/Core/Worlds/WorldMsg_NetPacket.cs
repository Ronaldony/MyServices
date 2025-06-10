using GameCore.NetPacket;

namespace Server_World.Core.Worlds
{
    /// <summary>
    /// WorldMsg_NetPacket.
    /// </summary>
    public class WorldMsg_NetPacket
    {
        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Packet.
        /// </summary>
        public NetPacketBase_CS Packet { get; set; }
    }
}
