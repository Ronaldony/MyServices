using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_World.Server.World.Handlers
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone;
    using Server_World.Core.Worlds.Msg;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_MoveStart : UdpHandler
    {
        private readonly ILogger<UdpHandler_MoveStart> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly WorldServer _worldServer;

        /// <summary>
        /// HuntzoneActor provider.
        /// </summary>
        private readonly WorldZoneActorProvider _worldZoneActorProvider;

        public UdpHandler_MoveStart(IServiceProvider serviceProvider)
            : base(NetPacketCode.CS_MoveStart)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_MoveStart>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _worldServer = serviceProvider.GetRequiredService<WorldServer>();

            _worldZoneActorProvider = serviceProvider.GetRequiredService<WorldZoneActorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var netPacket = _dataSerializer.Deserialize<NetPacketCS_MoveStart>(packet.Data);

            // Delivery msg.
            _worldZoneActorProvider.Tell<WorldMsg_NetPacket>(new WorldMsg_NetPacket
            {
                Pid = _worldServer.GetPid(sessionUid),
                PacketCode = NetPacketCode.CS_MoveStart,
                Packet = netPacket,
            });
        }
    }
}
