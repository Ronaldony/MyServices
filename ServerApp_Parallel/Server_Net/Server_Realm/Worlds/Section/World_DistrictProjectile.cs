namespace Server_Realm.Worlds.Section
{
    using Server_Realm.Projectiles.Enemies;
    using Server_Realm.Projectiles.Players;

    /// <summary>
    /// World_DistrictProjectile.
    /// </summary>
    public class World_DistrictProjectile : IDisposable
    {
        public World_DistrictProjectile()
        {
            Projectiles = new List<Projectile>();
            EnemyProjectiles = new List<EnemyProjectile>();
        }

        /// <summary>
        /// 플레이어 투사체.
        /// </summary>
        public List<Projectile> Projectiles { get; set; }

        /// <summary>
        /// 적 투사체.
        /// </summary>
        public List<EnemyProjectile> EnemyProjectiles { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Projectiles.Clear();
            EnemyProjectiles.Clear();
        }
    }
}
