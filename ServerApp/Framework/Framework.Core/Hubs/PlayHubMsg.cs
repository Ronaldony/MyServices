
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    using Framework.Core.Types;

    /// <summary>
    /// PlayHubMsg.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class PlayHubMsg
    {
        /// <summary>
        /// Type_PlayHubMsg.
        /// </summary>
        [MemoryPackOrder(0)]
        public Type_PlayHubMsg MsgType { get; set; }

        /// <summary>
        /// Msg.
        /// </summary>
        [MemoryPackOrder(1)]
        public byte[] Data { get; set; }
    }
}
