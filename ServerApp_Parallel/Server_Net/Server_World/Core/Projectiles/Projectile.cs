
using System.Numerics;

namespace Server_World.Core.Projectiles
{
    using GameCore;

    /// <summary>
    /// Projectile.
    /// </summary>
    public abstract class Projectile : IDisposable
    {
        /// <summary>
        /// Type_Projectile.
        /// </summary>
        public Type_Projectile ProjectilType => _projectileType;
        private readonly Type_Projectile _projectileType;

        /// <summary>
        /// 투사체 소유자.
        /// </summary>
        public string OwnerPid { get; set; }

        /// <summary>
        /// 투사체 시작 위치.
        /// </summary>
        public Vector2 StartPos { get; set; }
        
        /// <summary>
        /// 현재 위치.
        /// </summary>
        public Vector2 Pos { get; set; }
        
        /// <summary>
        /// 최대 길이.
        /// </summary>
        public int MaxRange { get; set; }

        /// <summary>
        /// 활성 여부.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 투사 각도.
        /// </summary>
        public Vector2 Angle { get; set; }

        /// <summary>
        /// 관통 여부.
        /// </summary>
        public bool IsPenetrates { get; set; }

        public Projectile(Type_Projectile projectile)
        {
            _projectileType = projectile;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
