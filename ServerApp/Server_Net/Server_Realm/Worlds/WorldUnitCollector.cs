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
            DeadUnits = new ConcurrentQueue<Unit>();
        }

        /// <summary>
        /// Dead units Q.
        /// </summary>
        public ConcurrentQueue<Unit> DeadUnits;

        /// <summary>
        /// Clears.
        /// </summary>
        public void Clears()
        {
            DeadUnits.Clear();
        }
    }
}

