
using Server_World.Core.Entities;
using System.Collections.Concurrent;

namespace Server_World.Core.Worlds.Section
{
    /// <summary>
    /// World_DistrictUnit.
    /// </summary>
    public class World_DistrictUnit : IDisposable
    {
        public World_DistrictUnit()
        {
            PlayerEntities = new ConcurrentBag<PlayerEntity>();
            EnemyEntities = new ConcurrentBag<EnemyEntity>();
        }

        /// <summary>
        /// WorldPlayers.
        /// </summary>
        public ConcurrentBag<PlayerEntity> PlayerEntities { get; set; }

        /// <summary>
        /// WorldEnemies.
        /// </summary>
        public ConcurrentBag<EnemyEntity> EnemyEntities { get; set; }

        public void Dispose()
        {
            PlayerEntities.Clear();
            EnemyEntities.Clear();
        }
    }
}
