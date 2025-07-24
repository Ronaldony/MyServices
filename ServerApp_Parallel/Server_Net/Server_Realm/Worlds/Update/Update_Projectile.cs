namespace Server_Realm.Worlds.Update
{
    using Server_Realm.Entities;
    using Server_Realm.Projectiles.Players;

    /// <summary>
    /// Update_Projectile.
    /// </summary>
    public class Update_Projectile : IDisposable
    {
        /// <summary>
        /// 제거 대상 투사체.
        /// </summary>
        public List<Projectile> RemoveProjectiles { get; set; }

        /// <summary>
        /// Death enemies.
        /// </summary>
        public List<EnemyUnit> DeathEnemies { get; set; }

        public Update_Projectile()
        {
            RemoveProjectiles = new List<Projectile>();
            DeathEnemies = new List<EnemyUnit>();
        }

        public void Dispose()
        {
            RemoveProjectiles.Clear();
            DeathEnemies.Clear();
        }
    }
}
