using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_AttackProjectile : GamePacketBase_CS
    {
        /// <summary>
        /// 투사체 스폰 지점.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 SpawnPos { get; set; }

        /// <summary>
        /// Angle.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Angle { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_AttackProjectile : GamePacketBase_SC
    {
        /// <summary>
        /// 공격 Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 투사체 스폰 지점.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 SpawnPos { get; set; }

        /// <summary>
        /// Angle.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 Angle { get; set; }

        /// <summary>
        /// ProjectileType.
        /// </summary>
        [MemoryPackOrder(3)]
        public Type_Projectile Projectile { get; set; }
    }
}
