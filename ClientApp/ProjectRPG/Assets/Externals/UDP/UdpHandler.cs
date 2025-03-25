
using ServerEngine.Network.UDP;

namespace Externals.UDP
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
        public abstract void HandlePacket(UdpPacket packet);

        /// <summary>
        /// Initialize packet code.
        /// </summary>
        public UdpHandler(int packetCode)
        {
            PacketCode = packetCode;
        }
    }
}
