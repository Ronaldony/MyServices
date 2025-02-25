
namespace ServerEngine.Network.Interfaces
{
    using ServerEngine.Network.UDP;

    /// <summary>
    /// IUdpServer.
    /// </summary>
    public interface IUdpServer
    {
        /// <summary>
        /// Start server.
        /// </summary>
        void Start(int port, bool allowPeerAddrChange, int pollingMilliseconds);

        /// <summary>
        /// Send.
        /// </summary>
        bool Send(IUdpSession session, UdpPacket packet);

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        bool BroadCast(IUdpSession session, UdpPacket packet);

        /// <summary>
        /// Get session.
        /// </summary>
        IUdpSession GetSessionOrDefault(int sessionId);

        /// <summary>
        /// Add white list.
        /// </summary>
        bool AddWhiteList(string address);

        /// <summary>
        /// Disconnect.
        /// </summary>
        bool Disconnect(IUdpSession session);
    }
}
