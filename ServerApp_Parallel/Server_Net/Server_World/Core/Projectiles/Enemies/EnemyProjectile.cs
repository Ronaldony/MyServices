using GameCore;

namespace Server_World.Core.Projectiles.Enemies
{
    /// <summary>
    /// EnemyProjectile.
    /// </summary>
    public abstract class EnemyProjectile : IDisposable
    {
        // Projectile type.
        private Type_EnemyProjectile _projectileType;
        public Type_EnemyProjectile ProjectileType => _projectileType;

        public EnemyProjectile(Type_EnemyProjectile enemyProjectile)
        { 
            
        }

        public abstract void Dispose();
    }
}
