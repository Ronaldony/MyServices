using MemoryPack;
using System.Runtime.InteropServices;
using System;
using System.Numerics;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_MoveStart.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_MoveStart : GamePacketBase_CS
    {
        /// <summary>
        /// Direction.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Direction { get; set; }

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
        /// Direction.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Direction { get; set; }
    }
}
