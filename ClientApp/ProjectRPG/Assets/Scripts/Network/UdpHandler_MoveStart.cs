using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{
    /// <summary>
    /// UdpHandler_MoveStart.
    /// </summary>
    public class UdpHandler_MoveStart : GamePacketHandler
    {
        private readonly GamePacketCS_MoveStart _sendPacket;

        public UdpHandler_MoveStart()
            : base(GamePacketCode.SC_MoveStart)
        {
            _sendPacket = new GamePacketCS_MoveStart();
        }

        /// <summary> 
        /// Handle packet.2
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
        }
    }
}
