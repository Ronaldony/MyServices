
using System.Net;
using System.Net.Sockets;

namespace ServerEngine.Test.Server
{
    using ServerEngine.Network.UDP;
    using ServerEngine.Test.Network;

    /// <summary>
    /// TestServer.
    /// </summary>
    public class TestServer : UdpServer<GameSession>
    {
        private readonly ILogger<UdpServer<GameSession>> _logger;

        public TestServer(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpServer<GameSession>>>();
        }

        /// <summary>
        /// OnRequestConnection.
        /// </summary>
        protected override void OnRequestConnection()
        {
            _logger.LogDebug($"[TestServer] Request connection.");
        }

        /// <summary>
        /// OnConnected.
        /// </summary>
        protected override GameSession OnConnected(int sessionId, IPAddress ipAddr)
        {
            var session = new GameSession
            {
                SessionId = sessionId,
                Address = ipAddr,
            };

            // TODO: Monitoring user indicator.

            return session;
        }

        /// <summary>
        /// OnDisconnected.
        /// </summary>
        protected override void OnDisconnected(GameSession session)
        {
            _logger.LogDebug($"[TestServer] Disconneted. sid: {session.SessionId}");

            // TODO: Monitoring user indicator.
        }

        /// <summary>
        /// OnNetError.
        /// </summary>
        protected override void OnNetError(SocketError sockError)
        {
            _logger.LogError($"[TestServer] Network error. error: {sockError}");
        }
    }
}