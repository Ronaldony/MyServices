using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_SpawnPlayer.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        public UdpHandler_SpawnPlayer()
            : base(NetPacketCode.SC_SpawnPlayer)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - SpawnPlayer");

            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_SpawnPlayer>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_SpawnPlayer,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
