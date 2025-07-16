using MemoryPack;
using System.Runtime.InteropServices;
using System;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_MoveStart.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_MoveStart : GamePacketBase_CS
    {
        /// <summary>
        /// MoveDirection.
        /// </summary>
        [MemoryPackOrder(0)]
        public Type_MoveDirection MoveDirection { get; set; }

        /// <summary>
        /// Packet time.
        /// </summary>
        [MemoryPackOrder(1)]
        public DateTime PacketTime { get; set; }
    }

    /// <summary>
    /// GamePacketSC_MoveStart.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_MoveStart : GamePacketBase_SC
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// MoveDirection.
        /// </summary>
        [MemoryPackOrder(1)]
        public Type_MoveDirection MoveDirection { get; set; }
    }
}
