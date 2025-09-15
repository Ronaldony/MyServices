
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_Knockback : GamePacketHandler
    {
        public UdpHandler_Knockback()
            : base(GamePacketCode.SC_KnockBack)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetKnockback= MemoryPackDataSerializer.Deserialize<GamePacketSC_KnockBack>(packet.Data);

            OpponentManagement.Instance.AddUpdatePacket(new UpdateGamePacket
            {
                Data = packetKnockback,
                PacketCode = packet.PacketCode,
            });
        }
    }
}
