
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
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
            : base(NetPacketCode.SC_SpawnEnemy)
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
            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_SpawnEnemy>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_SpawnEnemy,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
