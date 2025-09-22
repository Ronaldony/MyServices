using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone.Handlers
{
    using GameCore;
    using GameCore.GamePacket;
    using GameCore.HubPacket;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones.Providers;

    /// <summary>
    /// UdpHandler_ConsumeItem.
    /// </summary>
    public class UdpHandler_ConsumeItem : HuntZoneHandler
    {
        private readonly ILogger<UdpHandler_ConsumeItem> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_ConsumeItem(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_ConsumeItem)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_ConsumeItem>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var gamePacket = _dataSerializer.Deserialize<GamePacketCS_ConsumeItem>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<WorldMsg_GamePacket>(new WorldMsg_GamePacket
            {
                Pid = gamePacket.Pid,
                PacketCode = GamePacketCode.CS_ConsumeItem,
                Packet = gamePacket,
            });
        }
    }
}
