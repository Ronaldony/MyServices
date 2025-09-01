using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{
    /// <summary>
    /// UdpHandler_SpawnUnits.
    /// </summary>
    public class UdpHandler_SpawnUnits : GamePacketHandler
    {
        public UdpHandler_SpawnUnits()
            : base(GamePacketCode.SC_SpawnUnits)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_SpawnUnits>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_SpawnUnits,
                Data = packetData,
            };

            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
