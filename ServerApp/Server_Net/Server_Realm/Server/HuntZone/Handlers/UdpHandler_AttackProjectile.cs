using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.Network.UDP;

namespace Server_Realm.Server.HuntZone.Handlers
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones;

    /// <summary>
    /// UdpHandler_AttackMelee.
    /// </summary>
    public class UdpHandler_AttackProjectile : HuntZoneHandler
    {
        private readonly ILogger<UdpHandler_AttackProjectile> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneProcessorProvider _worldServerActorProvider;

        public UdpHandler_AttackProjectile(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_AttackProjectile)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_AttackProjectile>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _worldServerActorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            var gamePacket = _dataSerializer.Deserialize<GamePacketCS_AttackProjectile>(packet.Data);

            // Delivery msg.
            _worldServerActorProvider.Tell<WorldMsg_GamePacket>(new WorldMsg_GamePacket
            {
                Pid = gamePacket.Pid,
                PacketCode = GamePacketCode.CS_AttackProjectile,
                Packet = gamePacket,
            });
        }
    }
}
