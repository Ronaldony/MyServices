using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Server_World.Server.World
{
    using Akka.Actor;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// Game server.
    /// </summary>
    public sealed class WorldServer : UdpServer
    {
        private readonly ILogger<WorldServer> _logger;

        /// <summary>
        /// Sessoin dictionary.
        /// key: Session uid.
        /// value: PId.
        /// </summary>
        private ConcurrentDictionary<ulong, string> _sessionDict;

        public WorldServer(IServiceProvider serviceProvider)
            : base(serviceProvider, serviceProvider.GetRequiredService<ActorSystem>())
        {
            _logger = serviceProvider.GetService<ILogger<WorldServer>>();

            _sessionDict = new ConcurrentDictionary<ulong, string>();
        }

        /// <summary>
        /// OnConnected.
        /// </summary>
        protected override void OnConnected(int sessionId, ulong sessionUid)
        {
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
        /// Add pid.
        /// </summary>
        public bool RegisterPid(ulong sessionUid, string pid)
        {
            if (_sessionDict.ContainsKey(sessionUid))
            {
                return false;
            }

            _sessionDict.AddOrUpdate(sessionUid, pid, (key, old) => pid);

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
