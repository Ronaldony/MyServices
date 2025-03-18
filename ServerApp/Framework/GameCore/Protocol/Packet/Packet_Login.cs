using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Packet
{
    /// <summary>
    /// Packet_Login.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record Packet_Login
    {
        /// <summary>
        /// Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Uid { get; init; }

        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PId { get; init; }

        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(2)]
        public string Token { get; init; }
    }
}
