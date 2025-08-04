namespace Server_Realm.Worlds
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// WorldUnitCollector.
    /// </summary>
    public class WorldUnitCollector
    {
        public WorldUnitCollector()
        {
            DeadPlayerDict = new Dictionary<string, PlayerUnit>();
            DeadMobUnitDict = new Dictionary<long, MobUnit>();
        }

        /// <summary>
        /// Dead Player units.
        /// key: Pid.
        /// value: PlayerUnit.
        /// </summary>
        public Dictionary<string, PlayerUnit> DeadPlayerDict;

        /// <summary>
        /// Dead Mob units.
        /// key: UnitId.
        /// value: MobUnit.
        /// </summary>
        public Dictionary<long, MobUnit> DeadMobUnitDict;

        /// <summary>
        /// Add dead unit.
        /// </summary>
        public void AddDeadUnit(Unit unit)
        {
            switch (unit.UnitType)
            {
                case Type_Unit.Player:
                    var pu = unit as PlayerUnit;
                    DeadPlayerDict.Add(pu.Pid, pu);
                    break;
                case Type_Unit.Mob:
                    var mu = unit as MobUnit;
                    DeadMobUnitDict.Add(mu.UnitId, mu);
                    break;
            }
        }

        /// <summary>
        /// Clears.
        /// </summary>
        public void Clears()
        {
            DeadPlayerDict.Clear();
            DeadMobUnitDict.Clear();
        }
    }
}

