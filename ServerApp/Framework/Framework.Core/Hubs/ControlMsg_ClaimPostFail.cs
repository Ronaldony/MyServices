
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    /// <summary>
    /// ControlMsg_ClaimPostFail.
    /// Realm -> Control.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class ControlMsg_ClaimPostFail
    {
        public ControlMsg_ClaimPostFail()
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

        /// <summary>
        /// Fail count.
        /// </summary>
        [MemoryPackOrder(2)]
        public int FailCount { get; set; }
    }
}
