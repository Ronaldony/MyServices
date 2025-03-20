using GameCore.NetPacket;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Packet
{
    /// <summary>
    /// NetPacket_MoveStart from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_MoveStart : NetPacketBase
    {
        /// <summary>
        /// MoveDirection.
        /// </summary>
        [MemoryPackOrder(1)]
        public Type_MoveDirection MoveDirection { get; init; }

        /// <summary>
        /// Packet time.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime PacketTime { get; init; }
    }

    /// <summary>
    /// NetPacket_MoveStart from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_MoveStart : NetPacketBase
    {
        /// <summary>
        /// MoveDirection.
        /// </summary>
        [MemoryPackOrder(1)]
        public Type_MoveDirection MoveDirection { get; init; }

        /// <summary>
        /// Packet time.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime PacketTime { get; init; }
    }
}
