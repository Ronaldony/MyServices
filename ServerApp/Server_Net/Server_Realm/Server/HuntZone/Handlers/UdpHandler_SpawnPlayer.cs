using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone.Handlers
{
    using Framework.Core.Database.DAO;
    using GameCore;
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Play;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones;

    /// <summary>
    /// UdpHandler_LoadPlayer.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        private readonly ILogger<UdpHandler_SpawnPlayer> _logger;

        private readonly IUniqueIdService _uniqueIdService;
        private readonly UnitFactory _unitFactory;

        private readonly DBPlayInfo _dbPlayInfo;
        private readonly DBInventoryInfo _dbInventoryInfo;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_SpawnPlayer(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_SpawnUnits)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_SpawnPlayer>>();

            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();
            _unitFactory = serviceProvider.GetRequiredService<UnitFactory>();

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

                var pid = _huntZoneServer.GetPid(sessionUid);

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
                SpawnPlayerIntoWorld(sessionId, sessionUid, playInfo, inventoryInfo);
            });
        }

        /// <summary>
        /// Spawn player into world.
        /// </summary>
        private void SpawnPlayerIntoWorld(int sessionId, ulong sessionUid, DTO_PlayInfo playInfo, DTO_InventoryInfo inventoryInfo)
        {
            // Create player unit.
            var unitId = _uniqueIdService.GenerateIdAsLong();

            // Setup player unit.
            var pu = _unitFactory.CreateUnit<PlayerUnit>(Type_Unit.Player);
            pu.Setup(unitId, playInfo, inventoryInfo);

            // 메시지 전달.
            _worldServerActorProvider.Tell<WorldMsg_SpawnUnits>(new WorldMsg_SpawnUnits
            {
                PlayerUnit = pu
            });
        }
    }
}
