
using MemoryPack;
using System.Runtime.InteropServices;

namespace Framework.Core.Hubs
{
    using Framework.Core.Types;

    /// <summary>
    /// ControlMsg.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class ControlMsg
    {
        /// <summary>
        /// Type_ControlMsg.
        /// </summary>
        [MemoryPackOrder(0)]
        public Type_ControlMsg MsgType { get; set; }

        /// <summary>
        /// Msg.
        /// </summary>
        [MemoryPackOrder(1)]
        public byte[] Data { get; set; }
    }
}
