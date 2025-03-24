using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacketBase.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketBase
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }
    }
}
