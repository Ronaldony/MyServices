
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    /// <summary>
    /// ControlMsg_ClaimPostComplete.
    /// Realm -> Control.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class ControlMsg_ClaimPostComplete
    {
        public ControlMsg_ClaimPostComplete()
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
