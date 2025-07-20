
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_SpawnEnemy.
    /// </summary>
    public class UdpHandler_SpawnEnemy : UdpHandler
    {
        public UdpHandler_SpawnEnemy()
            : base(GamePacketCode.SC_SpawnEnemy)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - SC_SpawnEnemy");

            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_SpawnEnemy>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_SpawnEnemy,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
