
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    /// <summary>
    /// PlayHubMsg_FailClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class PlayHubMsg_FailClaimPost
    {
        public PlayHubMsg_FailClaimPost()
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Post uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }
    }
}
