using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{
    /// <summary>
    /// UdpHandler_DummyMove.
    /// </summary>
    public class UdpHandler_DummyMove : GamePacketHandler
    {
        public UdpHandler_DummyMove()
            : base(GamePacketCode.SC_DummyMove)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var dummyManagement = DummyManagement.Instance;

            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_DummyMove>(packet.Data);
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_DummyMove,
                Data = packetData,
            };

            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
