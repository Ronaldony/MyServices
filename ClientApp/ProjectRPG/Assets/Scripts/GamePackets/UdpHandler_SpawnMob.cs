
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{
    /// <summary>
    /// UdpHandler_SpawnEnemy.
    /// </summary>
    public class UdpHandler_SpawnMob : GamePacketHandler
    {
        public UdpHandler_SpawnMob()
            : base(GamePacketCode.SC_SpawnMob)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_SpawnMob>(packet.Data);
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_SpawnMob,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
