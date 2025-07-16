using System.Numerics;

namespace Server_World.Core.Projectiles
{
    using GameCore;

    /// <summary>
    /// Projectile.
    /// </summary>
    public abstract class EnemyProjectile : IDisposable
    {
        // Type_EnemyProjectile.
        public Type_EnemyProjectile ProjectilType => _projectileType;
        private readonly Type_EnemyProjectile _projectileType;

        public Vector2 StartPos { get; set; }

        public int MaxRange { get; set; }

        public EnemyProjectile(Type_EnemyProjectile projectile)
        {
            _projectileType = projectile;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
