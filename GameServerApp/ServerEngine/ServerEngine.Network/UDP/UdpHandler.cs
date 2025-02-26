
namespace ServerEngine.Network.UDP
{
    using ServerEngine.Network.Interfaces;

    /// <summary>
    /// UdpHandler.
    /// </summary>
    public abstract class UdpHandler
    {
        /// <summary>
        /// Packet code.
        /// </summary>
        public int PacketCode { get; set; }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public abstract void HandlePacket(UdpPacket packet, IUdpSession session, Func<int, ulong, UdpPacket, bool> sendFunc);

        /// <summary>
        /// Initialize packet code.
        /// </summary>
        public UdpHandler(int packetCode)
        {
            PacketCode = packetCode;
        }
    }
}
