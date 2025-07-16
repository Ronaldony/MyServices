using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_World.Server.World.Handlers
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Server.HuntZone;

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_KnockBack : UdpHandler
    {
        private readonly ILogger<UdpHandler_KnockBack> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneActorProvider _worldServerActorProvider;

        public UdpHandler_KnockBack(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_KnockBack)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_KnockBack>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var GamePacket = _dataSerializer.Deserialize<GamePacketCS_KnockBack>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<GameEvent>(new ReceivePacketEvent
            {
                Pid = _huntZoneServer.GetPid(sessionUid),
                PacketCode = GamePacketCode.CS_KnockBack,
                Packet = GamePacket,
            });
        }
    }
}
