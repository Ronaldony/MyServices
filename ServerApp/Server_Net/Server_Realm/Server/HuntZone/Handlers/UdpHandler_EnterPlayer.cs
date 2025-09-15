using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone.Handlers
{
    using Framework.Core.Database.DataObjects;
    using GameCore;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones.Providers;

    /// <summary>
    /// UdpHandler_EnterPlayer.
    /// </summary>
    public class UdpHandler_EnterPlayer : HuntZoneHandler
    {
        private readonly ILogger<UdpHandler_EnterPlayer> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly IUniqueIdService _uniqueIdService;
        private readonly UnitFactory _unitFactory;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_EnterPlayer(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_EnterPlayer)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_EnterPlayer>>();

            _serviceProvider = serviceProvider;
            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();
            _unitFactory = serviceProvider.GetRequiredService<UnitFactory>();

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

                var pid = _huntZoneServer.GetPid(sessionUid);

                ////////////////////////////////////////////////////////////////////
                /// Load info datas.

                // PlayInfo.
                var playInfoObject = _serviceProvider.GetRequiredService<PlayInfoObject>();
                await playInfoObject.LoadDataAsync(pid);

                if (null == playInfoObject.PlayInfo)
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // InventoryInfo.
                var inventoryInfoObject  = _serviceProvider.GetRequiredService<InventoryInfoObject>();
                await inventoryInfoObject.LoadDataAsync(pid);

                if (null == inventoryInfoObject.InventoryInfo)
                {
                    _huntZoneServer.Disconnect(sessionId, sessionUid);
                    return;
                }

                // Enter player into world.
                EnterPlayerIntoWorld(sessionId, sessionUid, playInfoObject, inventoryInfoObject);
            });
        }

        /// <summary>
        /// Enter player into world.
        /// </summary>
        private void EnterPlayerIntoWorld(int sessionId, ulong sessionUid, PlayInfoObject playInfoObject, InventoryInfoObject inventoryInfoObject)
        {
            // Create player unit.
            var unitId = _uniqueIdService.GenerateIdAsLong();

            // Setup player unit.
            var pu = _unitFactory.CreateUnit<PlayerUnit>(Type_Unit.Player);
            pu.Setup(unitId, playInfoObject.PlayInfo, inventoryInfoObject.InventoryInfo);

            // 월드 메시지.
            var worldMsg = new WorldMsg_EnterPlayer
            {
                PlayerUnit = pu
            };

            // Add DataObjects.
            worldMsg.DataObjects.Add(playInfoObject);
            worldMsg.DataObjects.Add(inventoryInfoObject);

            _worldServerActorProvider.Tell<WorldMsg_EnterPlayer>(worldMsg);
        }
    }
}
