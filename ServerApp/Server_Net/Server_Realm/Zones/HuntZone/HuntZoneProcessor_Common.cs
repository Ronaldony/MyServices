namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Context.Units;
    using Server_Realm.Entities;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Get player unit.
        /// </summary>
        public PlayerUnit GetPlayer(string pid)
        {
            if (_worldGroup.PlayerUnitDict.ContainsKey(pid))
            {
                return _worldGroup.PlayerUnitDict[pid];
            }

            return null;
        }

        /// <summary>
        /// Collect reusable unit.
        /// </summary>
        public void CollectReusableUnit(Unit unit)
        {
            _worldUnitCollector.ReusableUnits.Enqueue(unit);
        }
    }
}
