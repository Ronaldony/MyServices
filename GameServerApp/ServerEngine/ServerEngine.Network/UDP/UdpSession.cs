
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;

namespace ServerEngine.Network.UDP
{
    /// <summary>
    /// UdpSession.
    /// </summary>
    public class UdpSession
    {
        /// <summary>
        /// Session id.
        /// </summary>
        public int SessionId { get; set;  }

        /// <summary>
        /// Session address.
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// Peer.
        /// </summary>
        public NetPeer Peer { get; set; }

        public NetDataWriter DataWriter { get; set; }
    }
}
