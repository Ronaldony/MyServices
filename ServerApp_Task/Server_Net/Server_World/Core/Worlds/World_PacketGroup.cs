using Server_World.Core.Datas.GameEvents;
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
            PacketQueue = Channel.CreateUnbounded<NetPacketEvent>();
        }

        /// <summary>
        /// Pids.
        /// </summary>
        public HashSet<string> Pids { get; set; }

        /// <summary>
        /// Packet Q.
        /// </summary>
        public Channel<NetPacketEvent> PacketQueue { get; set; }
    }
}
