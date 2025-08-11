using Externals.UDP;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{

    /// <summary>
    /// UdpHandler_AttackMelee.
    /// </summary>
    public class UdpHandler_AttackMelee : UdpHandler
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
            Debug.Log("HandlePacket - AttackMelee");
        }
    }
}
