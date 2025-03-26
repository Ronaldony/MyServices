using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Server_World.Server.World.Handlers
{

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_MoveStart : UdpHandler
    {
        private readonly NetPacketCS_MoveStart _sendPacket;

        public UdpHandler_MoveStart()
            : base(GamePacketCode.SC_MoveStart)
        {
            _sendPacket = new NetPacketCS_MoveStart();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - MoveStart");
        }
    }
}
