using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Network.UDP;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Server_World.Server.HuntZone
{
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// HuntZoneServer.
    /// </summary>
    public sealed class HuntZoneServer : UdpServer
    {
        private readonly ILogger<HuntZoneServer> _logger;

        private readonly HuntZoneActorProvider _huntZoneProvider;

        /// <summary>
        /// Sessoin dictionary.
        /// key: Session uid.
        /// value: PId.
        /// </summary>
        private ConcurrentDictionary<ulong, string> _sessionDict;

        public HuntZoneServer(IServiceProvider serviceProvider)
            : base(serviceProvider, serviceProvider.GetRequiredService<ActorSystem>())
        {
            _logger = serviceProvider.GetService<ILogger<HuntZoneServer>>();

            _sessionDict = new ConcurrentDictionary<ulong, string>();

            _huntZoneProvider = serviceProvider.GetService<HuntZoneActorProvider>();
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
            // Remove session.
            _sessionDict.TryRemove(sessionUid, out var pid);

            // Send message.
            _huntZoneProvider.Tell<WorldMsg_OutPlayer>(new WorldMsg_OutPlayer
            {
                Pid = pid
            });
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
