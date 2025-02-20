
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
        void Start(int port);

        /// <summary>
        /// Send.
        /// </summary>
        bool Send(int sessionId, UdpPacket packet);

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        bool BroadCast(int sessionId, UdpPacket packet);

        /// <summary>
        /// Get session.
        /// </summary>
        IUdpSession GetSessionOrDefault(int sessionId);

        /// <summary>
        /// Add white list.
        /// </summary>
        bool AddWhiteList(string addressString);

        /// <summary>
        /// Disconnect.
        /// </summary>
        bool Disconnect(int sessionId);
    }
}
