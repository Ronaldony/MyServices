using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{

    /// <summary>
    /// UdpHandler_Dash.
    /// </summary>
    public class UdpHandler_Dash : GamePacketHandler
    {
        private readonly GamePacketCS_Dash _sendPacket;

        public UdpHandler_Dash()
            : base(GamePacketCode.SC_Dash)
        {
            _sendPacket = new GamePacketCS_Dash();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
        }
    }
}
