
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_UpdateMovePattern.
    /// </summary>
    public class UdpHandler_UpdateMovePattern : UdpHandler
    {
        public UdpHandler_UpdateMovePattern()
            : base(NetPacketCode.SC_UpdateMovePattern)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - SC_UpdateMovePattern");

            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_UpdateMovePattern>(packet.Data);
            var enemyManagement = EnemyManagement.Instance;

            enemyManagement.AddUpdatePacket(new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_UpdateMovePattern,
                Data = packetData,
            });
        }
    }
}
