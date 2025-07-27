using MemoryPack;
using System.Numerics;
using System.Reflection.Emit;
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
        /// Projectile id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int ProjectileId { get; set; }

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
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
