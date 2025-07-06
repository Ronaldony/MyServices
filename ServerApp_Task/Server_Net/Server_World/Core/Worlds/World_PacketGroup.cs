using System.Threading.Channels;

namespace Server_World.Core.Worlds
{
    /// <summary>
    /// World_PacketGroup.
    /// </summary>
    public class World_PacketGroup
    {
        public World_PacketGroup()
        {
            Pids = new HashSet<string>();
            PacketQueue = Channel.CreateUnbounded<World_NetPacket>();
        }

        /// <summary>
        /// Pids.
        /// </summary>
        public HashSet<string> Pids { get; set; }

        /// <summary>
        /// Packet Q.
        /// </summary>
        public Channel<World_NetPacket> PacketQueue { get; set; }
    }
}
