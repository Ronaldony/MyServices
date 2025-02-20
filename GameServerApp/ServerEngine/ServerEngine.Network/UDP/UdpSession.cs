
using LiteNetLib;
using LiteNetLib.Utils;
using ServerEngine.Network.Interfaces;
using System.Net;

namespace ServerEngine.Network.UDP
{
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

        /// <summary>
        /// Network data writer.
        /// </summary>
        public NetDataWriter DataWriter { get; set; }
    }
}
