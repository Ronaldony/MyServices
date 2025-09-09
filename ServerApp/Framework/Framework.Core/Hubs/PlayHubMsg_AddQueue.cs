using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    /// <summary>
    /// PlayHubMsg_AddQueue.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class PlayHubMsg_AddQueue
    {
        public PlayHubMsg_AddQueue()
        {

        }

        /// <summary>
        /// Queue.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Queue { get; set; }
    }
}
