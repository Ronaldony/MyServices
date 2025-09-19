
using Externals.Serializer;
using GameCore;
using GameCore.HubPacket;
using GamePackets;
using ServerEngine.Network.UDP;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_ClaimPost.
    /// </summary>
    public class UdpHandler_ClaimPost : GamePacketHandler
    {
        public UdpHandler_ClaimPost()
            : base(GamePacketCode.SC_ClaimPost)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetClaimPost = MemoryPackDataSerializer.Deserialize<GamePacketSC_ClaimPost>(packet.Data);

            // Add update pakcet.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = GamePacketCode.SC_ClaimPost,
                Data = packetClaimPost,
            });
        }
    }
}
