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
    [GamePacketAttribute(GamePacketCode.CS_MoveEnd)]
    public partial class GamePacketCS_MoveEnd : GamePacketBase_CS
    {
        /// <summary>
        /// Last position.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 EndPos { get; set; }

        /// <summary>
        /// PacketTime.
        /// </summary>
        [MemoryPackOrder(3)]
        public DateTime PacketTime { get; set; }
    }

    /// <summary>
    /// GamePacketSC_MoveEnd.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_MoveEnd)]
    public partial class GamePacketSC_MoveEnd : GamePacketBase_SC
    {
        /// <summary>
        /// MoveEnd unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// 마지막 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 EndPos { get; set; }
    }
}
