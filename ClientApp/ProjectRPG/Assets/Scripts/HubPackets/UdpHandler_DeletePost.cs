
using Externals.Serializer;
using GameCore;
using GameCore.HubPacket;
using ServerEngine.Network.UDP;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_DeletePost.
    /// </summary>
    public class UdpHandler_DeletePost : HubPacketHandler
    {
        public UdpHandler_DeletePost()
            : base(HubPacketCode.SC_DeletePost)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetDeletePost = MemoryPackDataSerializer.Deserialize<HubPacketSC_DeletePost>(packet.Data);
            var postInfo = PlayerDataManager.Instance.PostInfo;

            // Add update pakcet.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = HubPacketCode.SC_DeletePost,
                Data = packetDeletePost,
            });
        }
    }
}
