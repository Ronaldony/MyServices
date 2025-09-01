using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone.Handlers
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Server.HuntZone;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones;

    /// <summary>
    /// UdpHandler_Dash.
    /// </summary>
    public class UdpHandler_Dash : HuntZoneHandler
    {
        private readonly ILogger<UdpHandler_Dash> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_Dash(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_Dash)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Dash>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var gamePacket = _dataSerializer.Deserialize<GamePacketCS_Dash>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<WorldMsg_GamePacket>(new WorldMsg_GamePacket
            {
                Pid = gamePacket.Pid,
                PacketCode = GamePacketCode.CS_Dash,
                Packet = gamePacket,
            });
        }
    }
}
