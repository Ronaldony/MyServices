using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_Attack : GamePacketBase_CS
    {
        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Angle { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_Attack : GamePacketBase_SC
    {
        /// <summary>
        /// 공격 Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Angle { get; set; }
    }
}
