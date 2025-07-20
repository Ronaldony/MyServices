
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.GamePacket;
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
            : base(GamePacketCode.SC_UpdateMovePattern)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - SC_UpdateMovePattern");

            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_UpdateMovePattern>(packet.Data);
            var enemyManagement = EnemyManagement.Instance;

            enemyManagement.AddUpdatePacket(new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_UpdateMovePattern,
                Data = packetData,
            });
        }
    }
}
