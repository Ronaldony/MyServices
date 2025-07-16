using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;
using System;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketSC_MoveFixed.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_MoveFixed : GamePacketBase_SC
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 수정 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 FixedPos { get; set; }
    }
}
