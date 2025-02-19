
using ServerEngine.Network.Interfaces;
using System.Net;

namespace ServerEngine.Network.UDP
{
    using LiteNetLib;

    /// <summary>
    /// UdpSession.
    /// </summary>
    public class UdpSession : IUdpSession
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
    }
}
