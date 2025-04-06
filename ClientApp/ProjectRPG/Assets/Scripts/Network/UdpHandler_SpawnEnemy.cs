
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

            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_SpawnEnemy>(packet.Data);
            var enemyManagement = EnemyManagement.Instance;

            enemyManagement.AddUpdatePacket(new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_SpawnEnemy,
                Data = packetData,
            });
        }
    }
}
