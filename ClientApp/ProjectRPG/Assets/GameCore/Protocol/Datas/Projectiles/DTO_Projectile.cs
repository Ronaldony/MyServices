using MemoryPack;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Projectiles
{
    /// <summary>
    /// DTO_Projectile.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Projectile : IDisposable
    {
        /// <summary>
        /// Projectile type.
        /// </summary>
        [MemoryPackOrder(0)]
        public Type_Projectile ProjectileType { get; set; }

        /// <summary>
        /// 투사체 스폰 지점.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 SpawnPos { get; set; }

        /// <summary>
        /// 타겟 지점.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 TargetPos { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
