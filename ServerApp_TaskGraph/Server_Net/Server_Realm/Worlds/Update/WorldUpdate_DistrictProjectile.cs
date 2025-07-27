namespace Server_Realm.Worlds.Update
{
    using Server_Realm.Projectiles;
    using Server_Realm.Projectiles.Enemies;

    /// <summary>
    /// WorldUpdate_DistrictProjectile.
    /// </summary>
    public class WorldUpdate_DistrictProjectile : IDisposable
    {
        public WorldUpdate_DistrictProjectile()
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
