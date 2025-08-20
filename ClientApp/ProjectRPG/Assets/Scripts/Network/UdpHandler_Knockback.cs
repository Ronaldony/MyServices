using Externals.UDP;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{

    /// <summary>
    /// UdpHandler_Knockback.
    /// </summary>
    public class UdpHandler_Knockback : UdpHandler
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
