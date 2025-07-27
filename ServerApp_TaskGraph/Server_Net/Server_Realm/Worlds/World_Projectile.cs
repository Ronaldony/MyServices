namespace Server_Realm.Worlds
{
    using Server_Realm.Projectiles;
    using Server_Realm.Projectiles.Enemies;

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
