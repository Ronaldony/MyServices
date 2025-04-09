using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    using Server_World.Core.Worlds;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Database.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_LoadPlayer.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        private readonly ILogger<UdpHandler_SpawnPlayer> _logger;

        private readonly WorldServer _worldServer;

        // MemoryPoolService.
        private readonly IMemoryPoolService _memoryPoolService;

        /// <summary>
        /// Data object dictionary.
        /// key: Data object type.
        /// value: Data Object.
        /// </summary>
        private readonly Dictionary<Type, IDataObject> _dataObjectDict;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly HuntZoneActorProvider _huntZoneActorProvider;

        public UdpHandler_SpawnPlayer(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_SpawnPlayer)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_SpawnPlayer>>();

            _worldServer = serviceProvider.GetRequiredService<WorldServer>();

            // MemoryPoolService.
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

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
            _huntZoneActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
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

                // 게임 세션 저장.
                var gameSession = new GameSession
                {
                    SessionId = sessionId,
                    SessionUid = sessionUid
                };

                // Load player to hunt zone.
                LoadPlayerToHuntZone(pid, gameSession, playInfo);
            });
        }

        /// <summary>
        /// Load player to hunt zone actor.
        /// </summary>
        private void LoadPlayerToHuntZone(string pid, GameSession gameSession, DTO_PlayInfo playInfo)
        {
            // Actor msg.
            var actorMsg = new World_Player
            {
                PlayInfo = playInfo,
                GameSession = gameSession
            };

            // Delivery msg.
            _huntZoneActorProvider.Tell<World_Player>(actorMsg);
        }
    }
}
