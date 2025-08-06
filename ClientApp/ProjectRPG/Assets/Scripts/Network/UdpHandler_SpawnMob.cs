
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
    public class UdpHandler_SpawnMob : UdpHandler
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
            Debug.Log("HandlePacket - SC_SpawnEnemy");

            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_SpawnMob>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_SpawnMob,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
