
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
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
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_UpdateSectionNewEnter>(packet.Data);
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_UpdateSectionNewEnter,
                Data = packetData,
            };

            EnemyManagement.Instance.AddUpdatePacket(updatePacket);
            OpponentManagement.Instance.AddUpdatePacket(updatePacket);
        }
    }
}
