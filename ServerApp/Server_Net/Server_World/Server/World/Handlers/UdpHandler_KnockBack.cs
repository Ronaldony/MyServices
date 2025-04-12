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
    public class UdpHandler_KnockBack : UdpHandler
    {
        private readonly ILogger<UdpHandler_KnockBack> _logger;

        private readonly IDataSerializer _dataSerializer;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly HuntZoneActorProvider _huntZoneActorProvider;

        public UdpHandler_KnockBack(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_KnockBack)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_KnockBack>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _huntZoneActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            var netPacket = _dataSerializer.Deserialize<NetPacketCS_KnockBack>(packet.Data);

            // Delivery msg.
            _huntZoneActorProvider.Tell<NetPacketCS_KnockBack>(netPacket);
        }
    }
}
