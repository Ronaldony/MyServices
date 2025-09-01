using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{

    /// <summary>
    /// UdpHandler_Knockback.
    /// </summary>
    public class UdpHandler_Knockback : GamePacketHandler
    {
        private readonly GamePacketCS_KnockBack _sendPacket;

        public UdpHandler_Knockback()
            : base(GamePacketCode.SC_KnockBack)
        {
            _sendPacket = new GamePacketCS_KnockBack();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
        }
    }
}
