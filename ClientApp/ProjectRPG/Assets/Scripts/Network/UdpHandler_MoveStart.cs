using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_MoveStart.
    /// </summary>
    public class UdpHandler_MoveStart : UdpHandler
    {
        private readonly NetPacketCS_MoveStart _sendPacket;

        public UdpHandler_MoveStart()
            : base(NetPacketCode.SC_MoveStart)
        {
            _sendPacket = new NetPacketCS_MoveStart();
        }

        /// <summary> 
        /// Handle packet.2
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - MoveStart");
        }
    }
}
