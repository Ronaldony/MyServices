
using Externals.Serializer;
using GameCore;
using GameCore.HubPacket;
using ServerEngine.Network.UDP;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_ClaimPost.
    /// </summary>
    public class UdpHandler_ClaimPost : HubPacketHandler
    {
        public UdpHandler_ClaimPost()
            : base(HubPacketCode.SC_ClaimPost)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetClaimPost = MemoryPackDataSerializer.Deserialize<HubPacketSC_ClaimPost>(packet.Data);
            var postInfo = PlayerDataManager.Instance.PostInfo;

            // Add update pakcet.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = HubPacketCode.SC_ClaimPost,
                Data = packetClaimPost,
            });
        }
    }
}
