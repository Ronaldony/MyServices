using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_AttackMelee.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_AttackMelee)]
    public partial class GamePacketCS_AttackMelee : GamePacketBase_CS
    {
        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 Angle { get; set; }
    }

    /// <summary>
    /// GamePacketSC_AttackMelee.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_AttackMelee)]
    public partial class GamePacketSC_AttackMelee : GamePacketBase_SC
    {
        /// <summary>
        /// 공격 Unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Angle { get; set; }
    }
}
