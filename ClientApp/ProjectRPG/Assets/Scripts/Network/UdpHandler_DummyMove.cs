using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_DummyMove.
    /// </summary>
    public class UdpHandler_DummyMove : UdpHandler
    {
        public UdpHandler_DummyMove()
            : base(NetPacketCode.SC_DummyMove)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var dummyManagement = DummyManagement.Instance;

            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_DummyMove>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = NetPacketCode.SC_DummyMove,
                Data = packetData,
            };

            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
