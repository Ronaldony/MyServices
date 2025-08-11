namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;

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
    }
}
