
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{

    /// <summary>
    /// UdpHandler_Leave.
    /// </summary>
    public class UdpHandler_Leave : GamePacketHandler
    {
        public UdpHandler_Leave()
            : base(GamePacketCode.SC_Leave)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetLeave = MemoryPackDataSerializer.Deserialize<GamePacketSC_Leave>(packet.Data);

            // Leave player.
            OpponentManagement.Instance.DestroyOpponent(packetLeave.UnitId);
        }
    }
}
