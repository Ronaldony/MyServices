using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Server_Game.Server.Game
{
    using Server_Game.Core.Utilities;
    using ServerEngine.Network.UDP;
    using System.Collections.Concurrent;

    /// <summary>
    /// Game server.
    /// </summary>
    public sealed class GameServer : UdpServer
    {
        private readonly ILogger<GameServer> _logger;

        /// <summary>
        /// Sessoin dictionary.
        /// key: session uid.
        /// value: authentication.
        /// </summary>
        private ConcurrentDictionary<ulong, bool> SessionAuthDict;

        private Lock _lock;

        public GameServer()
            : base(ServiceUtility.GetServiceProvider())
        {
            _logger = ServiceUtility.GetService<ILogger<GameServer>>();
        }

        protected override void OnConnected(int sessionId, ulong sessionUid)
        {
            // Add session uid.
            SessionAuthDict.AddOrUpdate(sessionUid, false, (old, isAuth) => false);
        }

        protected override void OnDisconnected(int sessionId, ulong sessionUid)
        {
            lock (_lock)
            {
                SessionAuthDict.TryRemove(sessionUid, out _);
            }
        }

        protected override void OnNetError(SocketError sockError)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Login session.
        /// </summary>
        public void LoginSession(ulong sessoinUid)
        {
            lock(_lock)
            {
                SessionAuthDict.AddOrUpdate(sessoinUid, true, (_, _) => false);
            }
        }

        /// <summary>
        /// Check login.
        /// </summary>
        public bool IsLogin(ulong sessionUid)
        {
            if (SessionAuthDict.ContainsKey(sessionUid))
            {
                return true;
            }

            return false;
        }
    }
}
