using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_World.Server.World.Handlers
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_Dash : UdpHandler
    {
        private readonly ILogger<UdpHandler_Dash> _logger;

        private readonly IDataSerializer _dataSerializer;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly HuntZoneActorProvider _huntZoneActorProvider;

        public UdpHandler_Dash(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_Dash)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Dash>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _huntZoneActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            var netPacket = _dataSerializer.Deserialize<NetPacketCS_Dash>(packet.Data);

            // Delivery msg.
            _huntZoneActorProvider.Tell<NetPacketCS_Dash>(netPacket);
        }
    }
}
