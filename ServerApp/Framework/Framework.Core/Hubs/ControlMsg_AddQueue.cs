using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    /// <summary>
    /// ControlMsg_AddQueue.
    /// Realm -> Control.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class ControlMsg_AddQueue
    {
        public ControlMsg_AddQueue()
        {

        }

        /// <summary>
        /// Queue.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Queue { get; set; }
    }
}
