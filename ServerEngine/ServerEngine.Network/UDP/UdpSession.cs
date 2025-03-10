
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;

namespace ServerEngine.Network.UDP
{
    using ServerEngine.Network.Interfaces;

    /// <summary>
    /// UdpSession.
    /// </summary>
	public partial class UdpSession : IUdpSession
    {
        /// <summary>
        /// Session id.
        /// </summary>
		public int SessionId { get; set; }

        /// <summary>
        /// Disconnction check.
        /// </summary>
        public ulong SessionUid { get; set; }

        /// <summary>
        /// Disconnction check.
        /// </summary>
		public bool IsDisconn { get; set; }

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

        /// <summary>
        /// Network packet processor.
        /// </summary>
        public NetPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Packet.
        /// </summary>
        public UdpPacket Packet { get; set; }
    }
}
