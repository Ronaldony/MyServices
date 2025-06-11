using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Database.Interfaces;
using ServerEngine.Network.UDP;
using System.Reflection;

namespace Server_World.Server.World.Handlers
{
    using Framework.Core.Database.Attributes;
    using Framework.Core.Database.DataObject;
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Inventory;
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

        private readonly WorldServer _worldServer;

        /// <summary>
        /// Data object dictionary.
        /// key: Data object type.
        /// value: Data Object.
        /// </summary>
        private readonly Dictionary<Type, IDataObject> _dataObjectDict;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly WorldZoneActorProvider _worldZoneActorProvider;

        public UdpHandler_SpawnPlayer(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_SpawnPlayer)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_SpawnPlayer>>();

            _worldServer = serviceProvider.GetRequiredService<WorldServer>();

            // Data object 생성.
            _dataObjectDict = new Dictionary<Type, IDataObject>();

            // InfoObjectAttribute를 포함한 DataObject 가져오기.
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes().Where(d => d.GetCustomAttribute<DataInfoObjectAttribute>() != null))
                {
                    var dataObject = serviceProvider.GetRequiredService(type) as IDataObject;

                    _dataObjectDict.Add(type, dataObject);
                }
            }

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
                var playInfo = await _dataObjectDict[typeof(PlayInfoObject)].SelectAsync<DTO_PlayInfo>(pid);

                if (null == playInfo)
                {
                    _worldServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfo = await _dataObjectDict[typeof(InventoryInfoObject)].SelectAsync<DTO_InventoryInfo>(pid);

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
