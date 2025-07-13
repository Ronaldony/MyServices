
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_World.Server.World.Handlers
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.ActorProviders;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Server.HuntZone;

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_MoveEnd : UdpHandler
    {
        private readonly ILogger<UdpHandler_MoveEnd> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneActorProvider _worldServerActorProvider;

        public UdpHandler_MoveEnd(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_MoveEnd)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_MoveEnd>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
            var netPacket = _dataSerializer.Deserialize<NetPacketCS_MoveEnd>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<GameEvent>(new ReceivePacketEvent
            {
                Pid = _huntZoneServer.GetPid(sessionUid),
                PacketCode = NetPacketCode.CS_MoveEnd,
                Packet = netPacket,
            });
        }
    }
}
