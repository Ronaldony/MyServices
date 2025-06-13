using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Network.UDP;

namespace Server_World.Server.World.Handlers
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Play;
    using Server_World.Actors.HuntZone;
    using Server_World.Core.Services;
    using Server_World.Core.Worlds;

    /// <summary>
    /// UdpHandler_LoadPlayer.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        private readonly ILogger<UdpHandler_SpawnPlayer> _logger;

        private readonly DBPlayInfo _dbPlayInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        private readonly WorldServer _worldServer;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly WorldZoneActorProvider _worldZoneActorProvider;

        public UdpHandler_SpawnPlayer(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_SpawnPlayer)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_SpawnPlayer>>();

            _dbPlayInfo = serviceProvider.GetRequiredService<DBPlayInfo>();
            _dbInventoryInfo = serviceProvider.GetRequiredService<DBInventoryInfo>();

            _worldServer = serviceProvider.GetRequiredService<WorldServer>();

            // Actor 생성.
            _worldZoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            ThreadPool.QueueUserWorkItem(async param => 
            {
                _logger.LogDebug($"Player Spawn");

                var pid = _worldServer.GetPid(sessionUid);

                if (string.IsNullOrEmpty(pid))
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                ////////////////////////////////////////////////////////////////////
                /// Load info datas.

                // PlayInfo.
                var playInfo = await _dbPlayInfo.SelectAsync(pid);
                if (null == playInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfo = await _dbInventoryInfo.SelectAsync(pid);
                if (null == inventoryInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Load player to hunt zone.
                LoadPlayerToHuntZone(pid, sessionId, sessionUid, playInfo);
            });
        }

        /// <summary>
        /// Load player to hunt zone actor.
        /// </summary>
        private void LoadPlayerToHuntZone(string pid, int sessionId, ulong sessionUid, DTO_PlayInfo playInfo)
        {
            // 세션.
            var gameSession = new GameSession
            {
                SessionId = sessionId,
                SessionUid = sessionUid
            };

            // 충돌 박스.
            var collisionBox = CollisionManager.Rent(Type_Collision.Player);

            if (null == collisionBox)
            {
                return;
            }

            // 액터 메시지.
            var actorMsg = new World_Player
            {
                PlayInfo = playInfo,
                GameSession = gameSession,
                CollisionBox = collisionBox
            };

            // 메시지 전달.
            _worldZoneActorProvider.Tell<World_Player>(actorMsg);
        }
    }
}
