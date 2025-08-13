using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Network.UDP;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Server_Realm.Server.HuntZone
{
    using Framework.Core.Session;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// HuntZoneServer.
    /// </summary>
    public sealed class HuntZoneServer : UdpServer
    {
        private readonly ILogger<HuntZoneServer> _logger;

        private readonly HuntZoneProcessorProvider _huntZoneProvider;

        /// <summary>
        /// Sessoin dictionary.
        /// key: Session uid.
        /// value: PId.
        /// </summary>
        private ConcurrentDictionary<ulong, string> _sessionDict;

        /// <summary>
        /// GameSessoin dictionary.
        /// key: pid.
        /// value: game session.
        /// </summary>
        private ConcurrentDictionary<string, GameSession> _gameSessionDict;

        public HuntZoneServer(IServiceProvider serviceProvider)
            : base(serviceProvider, serviceProvider.GetRequiredService<ActorSystem>())
        {
            _logger = serviceProvider.GetService<ILogger<HuntZoneServer>>();

            _sessionDict = new ConcurrentDictionary<ulong, string>();
            _gameSessionDict = new ConcurrentDictionary<string, GameSession>();

            _huntZoneProvider = serviceProvider.GetService<HuntZoneProcessorProvider>();
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
            _gameSessionDict.TryRemove(pid, out _);

            // Send message.
            _huntZoneProvider.Tell<WorldMsg_LeavePlayer>(new WorldMsg_LeavePlayer
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
        public bool RegisterPid(int sessionId, ulong sessionUid, string pid)
        {
            if (_sessionDict.ContainsKey(sessionUid)    // 세션 중복 
                || _gameSessionDict.ContainsKey(pid))   // 이미 접속한 유저
            {
                return false;
            }

            var gameSession = new GameSession
            {
                SessionId = sessionId,
                SessionUid = sessionUid,
            };

            _sessionDict.AddOrUpdate(sessionUid, pid, (key, old) => pid);
            _gameSessionDict.AddOrUpdate(pid, gameSession, (key, old) => gameSession);

            return true;
        }

        /// <summary>
        /// Get pid.
        /// </summary>
        public GameSession GetGameSession(ulong sessionUid)
        {
            _sessionDict.TryGetValue(sessionUid, out var pid);
            _gameSessionDict.TryGetValue(pid, out var gameSession);

            return gameSession;
        }

        /// <summary>
        /// Get pid.
        /// </summary>
        public string GetPid(ulong sessionUid)
        {
            _sessionDict.TryGetValue(sessionUid, out var pid);

            return pid;
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(string pid, int packetCode, byte[] packetData)
        {
            if (false == _gameSessionDict.TryGetValue(pid, out var gameSession))
            {
                return false;
            }

            return Send(gameSession.SessionId, gameSession.SessionUid, packetCode, packetData);
        }
    }
}
