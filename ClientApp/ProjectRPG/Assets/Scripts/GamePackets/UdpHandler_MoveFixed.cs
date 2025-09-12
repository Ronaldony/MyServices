using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{

    /// <summary>
    /// UdpHandler_MoveFixed.
    /// </summary>
    public class UdpHandler_MoveFixed : GamePacketHandler
    {
        public UdpHandler_MoveFixed()
            : base(GamePacketCode.SC_MoveFixed)
        {
        }

        /// <summary> 
        /// Handle packet.2
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_MoveFixed>(packet.Data);

            if (packetData.UnitId == PlayerDataManager.Instance.UnitId)
            {
                PlayerController.Instance.AddUpdatePacket(new UpdateGamePacket
                {
                    PacketCode = GamePacketCode.SC_MoveFixed,
                    Data = packetData,
                });
            }
            else
            {
                OpponentManagement.Instance.AddUpdatePacket(new UpdateGamePacket
                {
                    PacketCode = GamePacketCode.SC_MoveFixed,
                    Data = packetData
                });
            }
        }
    }
}
