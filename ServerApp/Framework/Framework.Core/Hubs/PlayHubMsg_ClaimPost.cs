
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    using GameCore.Datas.Post;

    /// <summary>
    /// PlayHubMsg_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class PlayHubMsg_ClaimPost
    {
        public PlayHubMsg_ClaimPost()
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Zone id.
        /// </summary>
        [MemoryPackOrder(1)]
        public string ZoneId { get; set; }

        /// <summary>
        /// Post.
        /// </summary>
        [MemoryPackOrder(2)]
        public DTO_Post Post { get; set; }
    }
}
