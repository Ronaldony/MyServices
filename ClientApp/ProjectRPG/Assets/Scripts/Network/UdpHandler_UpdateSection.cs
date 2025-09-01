
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{
    /// <summary>
    /// UdpHandler_UpdateSection.
    /// </summary>
    public class UdpHandler_UpdateSection : GamePacketHandler
    {
        public UdpHandler_UpdateSection()
            : base(GamePacketCode.SC_UpdateSection)
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

            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_UpdateSection>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_UpdateSection,
                Data = packetData,
            };

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
