
using Externals.Serializer;
using GameCore;
using GameCore.HubPacket;
using ServerEngine.Network.UDP;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_PushPost.
    /// </summary>
    public class UdpHandler_PushPost : HubPacketHandler
    {
        public UdpHandler_PushPost()
            : base(HubPacketCode.SC_PushPost)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetPushPost = MemoryPackDataSerializer.Deserialize<HubPacketSC_PushPost>(packet.Data);
            var postInfo = PlayerDataManager.Instance.PostInfo;
            var exceptPostUids = new List<string>();

            foreach (var post in postInfo.Posts)
            {
                if (postInfo.Posts.ContainsKey(post.Key))
                {
                    exceptPostUids.Add(post.Key);
                    continue;
                }

                // Add posts.
                postInfo.Posts.Add(post.Key, post.Value);
            }

            // Except post uid.
            foreach (var postUid in exceptPostUids)
            {
                postInfo.Posts.Remove(postUid);
            }

            // Add update pakcet.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = HubPacketCode.SC_PushPost,
                Data = packetPushPost,
            });
        }
    }
}
