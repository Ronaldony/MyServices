
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

        /// <summary>
        /// Session id list.
        /// </summary>
        private readonly HashSet<int> _sessionIds;

        public TestServer(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpServer<GameSession>>>();

            _sessionIds = new HashSet<int>();
        }

        /// <summary>
        /// OnRequestConnection.
        /// </summary>
        protected override void OnRequestConnection()
        {
            _logger.LogInformation($"[TestServer] Request connection.");
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

            // Sesson duplicated.
            if (false == _sessionIds.Add(sessionId))
            {
                return null;
            }

            _logger.LogInformation($"[TestServer] Connection. sesion: {sessionId}");

            // TODO: Monitoring user indicator.

            return session;
        }

        /// <summary>
        /// OnDisconnected.
        /// </summary>
        protected override void OnDisconnected(GameSession session)
        {
            _logger.LogDebug($"[TestServer] Disconneted. sid: {session.SessionId}");

            if (false == _sessionIds.Remove(session.SessionId))
            {
                _logger.LogError($"[TestServer] Disconnection failed. sid: {session.SessionId}");
            }

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