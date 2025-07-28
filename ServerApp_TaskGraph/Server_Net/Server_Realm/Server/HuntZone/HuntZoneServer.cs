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
        private ConcurrentDictionary<ulong, long> _sessionDict;

        /// <summary>
        /// GameSessoin dictionary.
        /// key: pid.
        /// value: game session.
        /// </summary>
        private ConcurrentDictionary<long, GameSession> _gameSessionDict;

        public HuntZoneServer(IServiceProvider serviceProvider)
            : base(serviceProvider, serviceProvider.GetRequiredService<ActorSystem>())
        {
            _logger = serviceProvider.GetService<ILogger<HuntZoneServer>>();

            _sessionDict = new ConcurrentDictionary<ulong, long>();
            _gameSessionDict = new ConcurrentDictionary<long, GameSession>();

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
            _sessionDict.TryRemove(sessionUid, out var unitId);
            _gameSessionDict.TryRemove(unitId, out _);

            // Send message.
            _huntZoneProvider.Tell<WorldMsg_LeavePlayer>(new WorldMsg_LeavePlayer
            {
                UnitId = unitId
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
        public bool RegisterUnitId(int sessionId, ulong sessionUid, string pid, long unitId)
        {
            if (_sessionDict.ContainsKey(sessionUid)    // 세션 중복 
                || _gameSessionDict.ContainsKey(unitId))   // 이미 접속한 유저
            {
                return false;
            }

            var gameSession = new GameSession
            {
                SessionId = sessionId,
                SessionUid = sessionUid,
            };

            _sessionDict.AddOrUpdate(sessionUid, unitId, (key, old) => unitId);
            _gameSessionDict.AddOrUpdate(unitId, gameSession, (key, old) => gameSession);

            return true;
        }

        /// <summary>
        /// Get pid.
        /// </summary>
        public GameSession GetGameSession(ulong sessionUid)
        {
            _sessionDict.TryGetValue(sessionUid, out var unitId);
            _gameSessionDict.TryGetValue(unitId, out var gameSession);

            return gameSession;
        }

        /// <summary>
        /// Get pid.
        /// </summary>
        public long GetUnitId(ulong sessionUid)
        {
            _sessionDict.TryGetValue(sessionUid, out var unitId);

            return unitId;
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(long unitId, int packetCode, byte[] packetData)
        {
            if (_gameSessionDict.TryGetValue(unitId, out var gameSession))
            {
                return false;
            }

            return Send(gameSession.SessionId, gameSession.SessionUid, packetCode, packetData);
        }
    }
}
