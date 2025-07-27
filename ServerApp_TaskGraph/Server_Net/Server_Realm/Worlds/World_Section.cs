
namespace Server_Realm.Worlds
{
    using Server_Realm.Entities;
    using Server_Realm.Units.Enemy;

    /// <summary>
    /// World_Section.
    /// </summary>
    public class World_Section
    {
        public World_Section(SectionId section)
        {
            Section = section;
            _playerUnitDict = new Dictionary<string, PlayerUnit>();
            _mobUnitDict = new Dictionary<long, MobUnit>();
        }

        /// <summary>
        /// This section.
        /// </summary>
        public SectionId Section;

        /// <summary>
        /// Player unit.
        /// key: Pid.
        /// value: player unit.
        /// </summary>
        private Dictionary<string, PlayerUnit> _playerUnitDict;

        /// <summary>
        /// Mob unit.
        /// key: unit id.
        /// value: mob unit.
        /// </summary>
        private Dictionary<long, MobUnit> _mobUnitDict;

        // Get units.
        public IEnumerable<PlayerUnit> GetPlayUnits() => _playerUnitDict.Values;
        public IEnumerable<MobUnit> GetMobUnits() => _mobUnitDict.Values;

        /// <summary>
        /// Remove mob from section.
        /// </summary>
        public MobUnit LeaveMob(long unitId)
        {
            _mobUnitDict.Remove(unitId, out var mobUnit);
            return mobUnit;
        }

        /// <summary>
        /// Remove player from section.
        /// </summary>
        public PlayerUnit LeavePlayer(string pid)
        {
            _playerUnitDict.Remove(pid, out var playerUnit);
            return playerUnit;
        }

        /// <summary>
        /// Enter mob.
        /// </summary>
        public bool EnterMob(MobUnit mu)
        {
            return _mobUnitDict.TryAdd(mu.UnitId, mu);
        }

        /// <summary>
        /// Enter player.
        /// </summary>
        public bool EnterPlayer(PlayerUnit pu)
        {
            return _playerUnitDict.TryAdd(pu.Pid, pu);
        }
    }
}
