using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Packet
{
    /// <summary>
    /// Packet_Move.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record Packet_Move
    {
        /// <summary>
        /// Move.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Move { get; init; }

        /// <summary>
        /// Packet time.
        /// </summary>
        [MemoryPackOrder(1)]
        public DateTime PacketTime { get; init; }
    }
}
