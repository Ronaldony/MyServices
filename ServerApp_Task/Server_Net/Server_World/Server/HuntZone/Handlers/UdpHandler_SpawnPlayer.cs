using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Network.UDP;

namespace Server_World.Server.World.Handlers
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Play;
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Datas;
    using Server_World.Core.Services;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;
    using Server_World.Server.HuntZone;

    /// <summary>
    /// UdpHandler_LoadPlayer.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        private readonly ILogger<UdpHandler_SpawnPlayer> _logger;

        private readonly CollisionService _collisionService;

        private readonly DBPlayInfo _dbPlayInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneActorProvider _worldServerActorProvider;

        public UdpHandler_SpawnPlayer(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_SpawnPlayer)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_SpawnPlayer>>();

            _collisionService = serviceProvider.GetRequiredService<CollisionService>();

            _dbPlayInfo = serviceProvider.GetRequiredService<DBPlayInfo>();
            _dbInventoryInfo = serviceProvider.GetRequiredService<DBInventoryInfo>();

            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            // Actor 생성.
            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            Task.Run(async () => 
            {
                _logger.LogDebug($"Player Spawn");

                var pid = _huntZoneServer.GetPid(sessionUid);

                // check pid.
                if (string.IsNullOrEmpty(pid))
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                ////////////////////////////////////////////////////////////////////
                /// Load info datas.

                // PlayInfo.
                var playInfo = await _dbPlayInfo.SelectAsync(pid);
                if (null == playInfo)
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfo = await _dbInventoryInfo.SelectAsync(pid);
                if (null == inventoryInfo)
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Spawn player into world.
                SpawnPlayerIntoWorld(sessionId, sessionUid, playInfo);
            });
        }

        /// <summary>
        /// Spawn player into world.
        /// </summary>
        private void SpawnPlayerIntoWorld(int sessionId, ulong sessionUid, DTO_PlayInfo playInfo)
        {
            // 충돌 박스.
            var collisionBox = _collisionService.Rent(Type_Collision.Player);
            if (null == collisionBox)
            {
                return;
            }

            // 세션.
            var gameSession = new GameSession
            {
                SessionId = sessionId,
                SessionUid = sessionUid
            };

            var worldMsg = new WorldMsg_SpawnPlayer
            {
                WorldPlayer = new World_Player
                {
                    PlayInfo = playInfo,
                    GameSession = gameSession,
                    CollisionBox = collisionBox,
                    Section = SectionId.GetSection(playInfo.Location.Position),
                    Pid = playInfo.Pid
                }
            };

            // 메시지 전달.
            _worldServerActorProvider.Tell<WorldMsg>(worldMsg);
        }
    }
}
