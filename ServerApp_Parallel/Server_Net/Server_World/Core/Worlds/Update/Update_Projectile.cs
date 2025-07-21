
namespace Server_World.Core.Worlds.Update
{
    using Server_World.Core.Projectiles.Players;

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
        public List<World_Enemy> DeathEnemies { get; set; }

        public Update_Projectile()
        {
            RemoveProjectiles = new List<Projectile>();
            DeathEnemies = new List<World_Enemy>();
        }

        public void Dispose()
        {
            RemoveProjectiles.Clear();
            DeathEnemies.Clear();
        }
    }
}
