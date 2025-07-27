using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.World.Handlers
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.GameEvents;
    using Server_Realm.Server.HuntZone;

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_MoveStart : UdpHandler
    {
        private readonly ILogger<UdpHandler_MoveStart> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneServer _huntZonedServer;
        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_MoveStart(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_MoveStart)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_MoveStart>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _huntZonedServer = serviceProvider.GetRequiredService<HuntZoneServer>();

            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var GamePacket = _dataSerializer.Deserialize<GamePacketCS_MoveStart>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<GameEvent>(new ReceivePacketEvent
            {
                Pid = _huntZonedServer.GetPid(sessionUid),
                PacketCode = GamePacketCode.CS_MoveStart,
                Packet = GamePacket,
            });
        }
    }
}
