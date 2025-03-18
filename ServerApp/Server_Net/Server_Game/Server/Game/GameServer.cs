using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Server_Game.Server.Game
{
    using Server_Game.Core.Utilities;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// Game server.
    /// </summary>
    public sealed class GameServer : UdpServer
    {
        private readonly ILogger<GameServer> _logger;

        /// <summary>
        /// Sessoin dictionary.
        /// key: session uid.
        /// value: pid.
        /// </summary>
        private ConcurrentDictionary<ulong, string> _sessionDict;

        public GameServer()
            : base(ServiceUtility.GetServiceProvider())
        {
            _logger = ServiceUtility.GetService<ILogger<GameServer>>();
        }

        /// <summary>
        /// OnConnected.
        /// </summary>
        protected override void OnConnected(int sessionId, ulong sessionUid)
        {
            // Add session uid.
            _sessionDict.AddOrUpdate(sessionUid, string.Empty, (_, _) => string.Empty);
        }

        /// <summary>
        /// OnDisconnected.
        /// </summary>
        protected override void OnDisconnected(int sessionId, ulong sessionUid)
        {
            _sessionDict.TryRemove(sessionUid, out _);
        }

        /// <summary>
        /// OnNetError.
        /// </summary>
        protected override void OnNetError(SocketError sockError)
        {
            _logger.LogError($"GameServer error. code: {sockError}");
        }

        /// <summary>
        /// Register pid.
        /// </summary>
        public bool RegisterPid(ulong sessionUid, string pid)
        {
            if (false == _sessionDict.ContainsKey(sessionUid))
            {
                return false;
            }

            _sessionDict.AddOrUpdate(sessionUid, pid, (_, _) => pid);

            return true;
        }

        /// <summary>
        /// Get pid.
        /// </summary>
        public string GetPid(ulong sessionUid)
        {
            _sessionDict.TryGetValue(sessionUid, out var pid);

            return pid;
        }
    }
}
