
using Externals.Serializer;
using GameCore;
using GameCore.HubPacket;
using ServerEngine.Network.UDP;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_FailClaimPost.
    /// </summary>
    public class UdpHandler_ClaimPostFail : HubPacketHandler
    {
        public UdpHandler_ClaimPostFail()
            : base(HubPacketCode.SC_ClaimPostFail)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetFailClaimPost = MemoryPackDataSerializer.Deserialize<HubPacketSC_ClaimPostFail>(packet.Data);

            // Add update pakcet.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = HubPacketCode.SC_ClaimPostFail,
                Data = packetFailClaimPost,
            });
        }
    }
}
