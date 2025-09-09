using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{

    /// <summary>
    /// UdpHandler_MoveEnd.
    /// </summary>
    public class UdpHandler_MoveEnd : GamePacketHandler
    {
        private readonly GamePacketCS_MoveEnd _sendPacket;

        public UdpHandler_MoveEnd()
            : base(GamePacketCode.SC_MoveEnd)
        {
            _sendPacket = new GamePacketCS_MoveEnd();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
        }
    }
}
