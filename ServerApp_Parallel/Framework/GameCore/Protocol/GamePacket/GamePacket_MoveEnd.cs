using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;
using System;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_MoveEnd.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_MoveEnd : GamePacketBase_CS
    {
        /// <summary>
        /// Last position.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 EndPos { get; set; }

        /// <summary>
        /// PacketTime.
        /// </summary>
        [MemoryPackOrder(1)]
        public DateTime PacketTime { get; set; }
    }

    /// <summary>
    /// GamePacketSC_MoveEnd.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_MoveEnd : GamePacketBase_SC
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 마지막 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 EndPos { get; set; }
    }
}
