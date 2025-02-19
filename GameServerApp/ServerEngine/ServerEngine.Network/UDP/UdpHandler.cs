
namespace ServerEngine.Network.UDP
{
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
        public abstract void HandlePacket<TSession>(UdpPacket packet, TSession session);

        /// <summary>
        /// Initialize packet code.
        /// </summary>
        public UdpHandler(int packetCode)
        {
            PacketCode = packetCode;
        }
    }
}
