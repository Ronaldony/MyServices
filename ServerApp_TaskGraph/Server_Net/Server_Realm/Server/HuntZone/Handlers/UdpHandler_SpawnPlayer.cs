using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.World.Handlers
{
    using Framework.Core.Database.DAO;
    using GameCore;
    using GameCore.Datas.Play;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Core.Factory;
    using Server_Realm.Entities;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// UdpHandler_LoadPlayer.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        private readonly ILogger<UdpHandler_SpawnPlayer> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly CollisionFactory _collisionService;

        private readonly DBPlayInfo _dbPlayInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_SpawnPlayer(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_SpawnPlayer)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_SpawnPlayer>>();

            _serviceProvider = serviceProvider;
            _collisionService = serviceProvider.GetRequiredService<CollisionFactory>();

            _dbPlayInfo = serviceProvider.GetRequiredService<DBPlayInfo>();
            _dbInventoryInfo = serviceProvider.GetRequiredService<DBInventoryInfo>();

            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            // Actor 생성.
            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            Task.Run(async () => 
            {
                _logger.LogDebug($"Player Spawn");

                var unitId = _huntZoneServer.GetUnitId(sessionUid);
                var gameSession = _huntZoneServer.GetGameSession(sessionUid);

                ////////////////////////////////////////////////////////////////////
                /// Load info datas.

                // PlayInfo.
                var playInfo = await _dbPlayInfo.SelectAsync(gameSession.Pid);
                if (null == playInfo)
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfo = await _dbInventoryInfo.SelectAsync(gameSession.Pid);
                if (null == inventoryInfo)
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Spawn player into world.
                SpawnPlayerIntoWorld(sessionId, sessionUid, unitId, playInfo);
            });
        }

        /// <summary>
        /// Spawn player into world.
        /// </summary>
        private void SpawnPlayerIntoWorld(int sessionId, ulong sessionUid, long unitId, DTO_PlayInfo playInfo)
        {
            // 충돌 박스.
            var playerColllision = _collisionService.Create(Type_Collision.Player);
            if (null == playerColllision)
            {
                return;
            }

            // Create player unit.
            var playerUnit = new PlayerUnit(_serviceProvider, unitId, playInfo, playerColllision);

            // 메시지 전달.
            _worldServerActorProvider.Tell<WorldMsg_SpawnPlayer>(new WorldMsg_SpawnPlayer
            {
                PlayerUnit = playerUnit
            });
        }
    }
}
