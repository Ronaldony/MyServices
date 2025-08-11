
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Projectiles;

    /// <summary>
    /// GamePacketCS_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_AttackProjectile)]
    public partial class GamePacketCS_AttackProjectile : GamePacketBase_CS
    {
        /// <summary>
        /// Projectile.
        /// </summary>
        [MemoryPackOrder(2)]
        public DTO_Projectile Projectile { get; set; }
    }

    /// <summary>
    /// GamePacketSC_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_AttackProjectile)]
    public partial class GamePacketSC_AttackProjectile : GamePacketBase_SC
    {
        /// <summary>
        /// 공격 Unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// Projectile.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_Projectile Projectile { get; set; }
    }
}
