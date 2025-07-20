namespace Server_World.Core.Worlds
{
    using Server_World.Core.Projectiles.Enemies;
    using Server_World.Core.Projectiles.Players;

    /// <summary>
    /// World_Projectile.
    /// </summary>
    public class World_Projectile
    {
        public World_Projectile() 
        {
            Projectiles = new HashSet<Projectile>();
            SpawnProjectiles = new List<Projectile>();
            EnemyProjectiles = new HashSet<EnemyProjectile>();
        }

        /// <summary>
        /// 투사체 목록.
        /// </summary>
        public HashSet<Projectile> Projectiles { get; set; }

        /// <summary>
        /// 스폰 투사체 목록.
        /// </summary>
        public List<Projectile> SpawnProjectiles { get; set; }

        /// <summary>
        /// 적 유닛 투사체 목록.
        /// </summary>
        public HashSet<EnemyProjectile> EnemyProjectiles { get; set; }
    }
}
