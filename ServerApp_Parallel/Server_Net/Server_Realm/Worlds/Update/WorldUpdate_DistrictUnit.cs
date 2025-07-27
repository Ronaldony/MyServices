
using System.Collections.Concurrent;

namespace Server_Realm.Worlds.Update
{
    using Server_Realm.Entities;
    using Server_Realm.Units.Enemy;

    /// <summary>
    /// WorldUpdate_DistrictUnit.
    /// </summary>
    public class WorldUpdate_DistrictUnit : IDisposable
    {
        public WorldUpdate_DistrictUnit()
        {
            PlayerUnits = new ConcurrentBag<PlayerUnit>();
            MobUnits = new ConcurrentBag<MobUnit>();
        }

        /// <summary>
        /// WorldPlayers.
        /// </summary>
        public ConcurrentBag<PlayerUnit> PlayerUnits { get; set; }

        /// <summary>
        /// WorldEnemies.
        /// </summary>
        public ConcurrentBag<MobUnit> MobUnits { get; set; }

        public void Dispose()
        {
            PlayerUnits.Clear();
            MobUnits.Clear();
        }
    }
}
