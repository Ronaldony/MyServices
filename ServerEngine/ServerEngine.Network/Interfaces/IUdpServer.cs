
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
        void Start(int port, bool allowPeerAddrChange);

        /// <summary>
        /// Send.
        /// </summary>
        bool Send(int sessionId, ulong sessionUid, int packetCode, byte[] packetData);

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        bool BroadCast(int sessionId, ulong sessionUid, int packetCode, byte[] packetData);

        /// <summary>
        /// Add white list.
        /// </summary>
        bool AddWhiteList(string address);

        /// <summary>
        /// Disconnect.
        /// </summary>
        void Disconnect(int sessionId, ulong sessionUid);
    }
}
