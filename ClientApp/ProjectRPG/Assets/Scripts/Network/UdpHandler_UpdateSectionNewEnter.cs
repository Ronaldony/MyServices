
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{
    /// <summary>
    /// UdpHandler_UpdateSectionNewEnter.
    /// </summary>
    public class UdpHandler_UpdateSectionNewEnter : GamePacketHandler
    {
        public UdpHandler_UpdateSectionNewEnter()
            : base(GamePacketCode.SC_UpdateSectionNewEnter)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // management.
            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_UpdateSectionNewEnter>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_UpdateSectionNewEnter,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
