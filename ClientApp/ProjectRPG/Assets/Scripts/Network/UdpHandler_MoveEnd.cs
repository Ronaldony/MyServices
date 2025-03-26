using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Server_World.Server.World.Handlers
{

    /// <summary>
    /// UdpHandler_MoveEnd.
    /// </summary>
    public class UdpHandler_MoveEnd : UdpHandler
    {
        private readonly NetPacketCS_MoveEnd _sendPacket;

        public UdpHandler_MoveEnd()
            : base(NetPacketCode.SC_MoveStart)
        {
            _sendPacket = new NetPacketCS_MoveEnd();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - MoveEnd");
        }
    }
}
