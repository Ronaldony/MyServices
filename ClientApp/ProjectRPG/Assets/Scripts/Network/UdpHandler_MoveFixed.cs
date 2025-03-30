using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{

    /// <summary>
    /// UdpHandler_MoveFixed.
    /// </summary>
    public class UdpHandler_MoveFixed : UdpHandler
    {
        public UdpHandler_MoveFixed()
            : base(NetPacketCode.SC_MoveFixed)
        {
        }

        /// <summary> 
        /// Handle packet.2
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - MoveFixed");

            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_MoveFixed>(packet.Data);

            var playerController = PlayerController.Instance;
            playerController.EnqueueUpdatePacket(new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_MoveFixed,
                Data = packetData,
            });
        }
    }
}
