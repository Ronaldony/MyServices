using Server_Realm.Entities;
using System.Collections.Concurrent;

namespace Server_Realm.Worlds.Section
{
    /// <summary>
    /// World_DistrictUnit.
    /// </summary>
    public class World_DistrictUnit : IDisposable
    {
        public World_DistrictUnit()
        {
            PlayerEntities = new ConcurrentBag<PlayerUnit>();
            EnemyEntities = new ConcurrentBag<EnemyUnit>();
        }

        /// <summary>
        /// WorldPlayers.
        /// </summary>
        public ConcurrentBag<PlayerUnit> PlayerEntities { get; set; }

        /// <summary>
        /// WorldEnemies.
        /// </summary>
        public ConcurrentBag<EnemyUnit> EnemyEntities { get; set; }

        public void Dispose()
        {
            PlayerEntities.Clear();
            EnemyEntities.Clear();
        }
    }
}
