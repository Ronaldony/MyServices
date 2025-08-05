namespace Server_Realm.Worlds
{
    using Server_Realm.Units.Mob;

    /// <summary>
    /// WorldUnitCollector.
    /// </summary>
    public class WorldUnitCollector
    {
        public WorldUnitCollector()
        {
            DeadMobUnitDict = new Dictionary<long, MobUnit>();
        }

        /// <summary>
        /// Dead Mob units.
        /// key: UnitId.
        /// value: MobUnit.
        /// </summary>
        public Dictionary<long, MobUnit> DeadMobUnitDict;

        /// <summary>
        /// Clears.
        /// </summary>
        public void Clears()
        {
            DeadMobUnitDict.Clear();
        }
    }
}

