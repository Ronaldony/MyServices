
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_UpdateSection.
    /// </summary>
    public class UdpHandler_UpdateSection : UdpHandler
    {
        public UdpHandler_UpdateSection()
            : base(NetPacketCode.SC_UpdateSection)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - SC_UpdateSection");

            // management.
            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_UpdateSection>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_UpdateSection,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
