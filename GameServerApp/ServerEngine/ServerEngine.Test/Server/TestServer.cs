
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

		protected override void OnConnected(ref GameSession session, IPAddress ipAddr)
        {
            // initialize session.
			session.Uid = string.Empty;
			session.Pid = string.Empty;

            // Sesson duplicated.
            if (false == _sessionIds.Add(session.SessionId))
            {
                return;
            }

            // TODO: Monitoring user indicator.
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