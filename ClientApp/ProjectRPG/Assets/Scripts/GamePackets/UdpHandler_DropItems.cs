using Externals.Serializer;
using ServerEngine.Network.UDP;

namespace GamePackets
{
    using GameCore;
    using GameCore.GamePacket;

    /// <summary>
    /// UdpHandler_DropItems.
    /// </summary>
    public class UdpHandler_DropItems : GamePacketHandler
    {
        public UdpHandler_DropItems()
            : base(GamePacketCode.SC_DropItems)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_DropItems>(packet.Data);
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_DropItems,
                Data = packetData,
            };

            var enemyManagement = EnemyManagement.Instance;
            enemyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
