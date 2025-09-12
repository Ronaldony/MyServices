using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{

    /// <summary>
    /// UdpHandler_AttackMelee.
    /// </summary>
    public class UdpHandler_AttackMelee : GamePacketHandler
    {
        private readonly GamePacketCS_AttackMelee _sendPacket;

        public UdpHandler_AttackMelee()
            : base(GamePacketCode.SC_AttackMelee)
        {
            _sendPacket = new GamePacketCS_AttackMelee();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_AttackMelee>(packet.Data);

            OpponentManagement.Instance.AddUpdatePacket(new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_AttackMelee,
                Data = packetData
            });
        }
    }
}
