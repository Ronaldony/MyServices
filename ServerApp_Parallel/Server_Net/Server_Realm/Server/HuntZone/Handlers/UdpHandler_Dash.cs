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
    /// UdpHandler_Dash.
    /// </summary>
    public class UdpHandler_Dash : UdpHandler
    {
        private readonly ILogger<UdpHandler_Dash> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneServer _huntZoneServer;
        private readonly HuntZoneActorProvider _worldServerActorProvider;

        public UdpHandler_Dash(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_Dash)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Dash>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _huntZoneServer = serviceProvider.GetRequiredService<HuntZoneServer>();
            
            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var GamePacket = _dataSerializer.Deserialize<GamePacketCS_Dash>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<GameEvent>(new ReceivePacketEvent
            {
                Pid = _huntZoneServer.GetPid(sessionUid),
                PacketCode = GamePacketCode.CS_Dash,
                Packet = GamePacket,
            });
        }
    }
}
