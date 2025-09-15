using System.Collections.Concurrent;

namespace Server_Realm.Worlds
{
    using Server_Realm.Context.Units;

    /// <summary>
    /// WorldUnitCollector.
    /// </summary>
    public class WorldUnitCollector
    {
        public WorldUnitCollector()
        {
            ReusableUnits = new ConcurrentQueue<Unit>();
        }

        /// <summary>
        /// Reusable units.
        /// </summary>
        public ConcurrentQueue<Unit> ReusableUnits;
    }
}

