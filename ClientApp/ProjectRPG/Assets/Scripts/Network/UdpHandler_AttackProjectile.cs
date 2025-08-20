using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_AttackProjectile.
    /// </summary>
    public class UdpHandler_AttackProjectile : UdpHandler
    {
        private readonly GamePacketCS_AttackProjectile _sendPacket;

        public UdpHandler_AttackProjectile()
            : base(GamePacketCode.SC_AttackProjectile)
        {
            _sendPacket = new GamePacketCS_AttackProjectile();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // management.
            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_AttackProjectile>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_AttackProjectile,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
