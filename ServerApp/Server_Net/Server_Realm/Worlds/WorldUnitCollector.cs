namespace Server_Realm.Worlds
{
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;
    using System.Collections.Concurrent;

    /// <summary>
    /// WorldUnitCollector.
    /// </summary>
    public class WorldUnitCollector
    {
        public WorldUnitCollector()
        {
            DeadUnits = new ConcurrentDictionary<long, Unit>();
        }

        /// <summary>
        /// Dead Mob units.
        /// key: UnitId.
        /// value: MobUnit.
        /// </summary>
        public ConcurrentDictionary<long, Unit> DeadUnits;

        /// <summary>
        /// Clears.
        /// </summary>
        public void Clears()
        {
            DeadUnits.Clear();
        }
    }
}

