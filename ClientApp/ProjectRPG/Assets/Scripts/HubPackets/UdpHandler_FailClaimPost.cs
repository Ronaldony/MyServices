
using Externals.Serializer;
using GameCore;
using GameCore.HubPacket;
using ServerEngine.Network.UDP;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_FailClaimPost.
    /// </summary>
    public class UdpHandler_FailClaimPost : HubPacketHandler
    {
        public UdpHandler_FailClaimPost()
            : base(HubPacketCode.SC_FailClaimPost)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetFailClaimPost = MemoryPackDataSerializer.Deserialize<HubPacketSC_FailClaimPost>(packet.Data);
            var postInfo = PlayerDataManager.Instance.PostInfo;

            // Add update pakcet.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = HubPacketCode.SC_FailClaimPost,
                Data = packetFailClaimPost,
            });
        }
    }
}
