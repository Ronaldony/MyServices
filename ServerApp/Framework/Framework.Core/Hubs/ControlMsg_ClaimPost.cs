
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    using GameCore.Datas.Post;

    /// <summary>
    /// ControlMsg_ClaimPost.
    /// Control -> Realm.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class ControlMsg_ClaimPost
    {
        public ControlMsg_ClaimPost()
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

        /// <summary>
        /// Try count.
        /// </summary>
        [MemoryPackOrder(3)]
        public int TryCount { get; set; }
    }
}
