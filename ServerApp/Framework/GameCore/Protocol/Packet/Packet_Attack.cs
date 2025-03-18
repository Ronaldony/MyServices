using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Packet
{
    /// <summary>
    /// Packet_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record Packet_Attack
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; init; }

        /// <summary>
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int AttackId { get; init; }
    }
}
