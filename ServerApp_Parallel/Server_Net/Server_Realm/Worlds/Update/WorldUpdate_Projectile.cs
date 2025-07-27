namespace Server_Realm.Worlds.Update
{
    using Server_Realm.Projectiles.Players;
    using Server_Realm.Units.Enemy;

    /// <summary>
    /// WorldUpdate_Projectile.
    /// </summary>
    public class WorldUpdate_Projectile : IDisposable
    {
        /// <summary>
        /// 제거 대상 투사체.
        /// </summary>
        public List<Projectile> RemoveProjectiles { get; set; }

        /// <summary>
        /// Death enemies.
        /// </summary>
        public List<MobUnit> DeathEnemies { get; set; }

        public WorldUpdate_Projectile()
        {
            RemoveProjectiles = new List<Projectile>();
            DeathEnemies = new List<MobUnit>();
        }

        public void Dispose()
        {
            RemoveProjectiles.Clear();
            DeathEnemies.Clear();
        }
    }
}
