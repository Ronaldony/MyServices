using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Core.Services.Interfaces;
using System.Collections.Concurrent;
using System.Numerics;

namespace Server_Realm.Worlds
{
    using GameCore;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;
    using Server_Realm.Zones;

    /// <summary>
    /// WorldSection.
    /// </summary>
    public partial class WorldSection
    {
        public WorldSection(IServiceProvider serviceProvider, IZoneProcessor zoneProcessor, SectionId section)
        {
            _zoneProcessor = zoneProcessor;
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            Section = section;
            _playerUnitDict = new ConcurrentDictionary<long, PlayerUnit>();
            _mobUnitDict = new ConcurrentDictionary<long, MobUnit>();
            _aroundWorldSections = new List<WorldSection>();
        }

        /// <summary>
        /// This section.
        /// </summary>
        public SectionId Section { get; private set; }

        // Get units.
        public IEnumerable<PlayerUnit> GetPlayerUnits() => _playerUnitDict.Values;
        public IEnumerable<MobUnit> GetMobUnits() => _mobUnitDict.Values;

        /// <summary>
        /// ZoneProcessor.
        /// </summary>
        private readonly IZoneProcessor _zoneProcessor;

        /// <summary>
        /// DataSerializer.
        /// </summary>
        private readonly IDataSerializer _dataSerializer;

        /// <summary>
        /// Player unit.
        /// key: Pid.
        /// value: player unit.
        /// </summary>
        private ConcurrentDictionary<long, PlayerUnit> _playerUnitDict;

        /// <summary>
        /// Mob unit.
        /// key: unit id.
        /// value: mob unit.
        /// </summary>
        private ConcurrentDictionary<long, MobUnit> _mobUnitDict;

        /// <summary>
        /// Around sections.
        /// </summary>
        private List<WorldSection> _aroundWorldSections;

        /// <summary>
        /// Add around world section.
        /// </summary>
        public void AddAroundWorldSection(WorldSection worldSection)
        {
            _aroundWorldSections.Add(worldSection);
        }

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
        public PlayerUnit LeavePlayer(long unitId)
        {
            _playerUnitDict.Remove(unitId, out var playerUnit);
            return playerUnit;
        }

        /// <summary>
        /// Enter mob.
        /// </summary>
        public void EnterMob(MobUnit mu)
        {
            _mobUnitDict.AddOrUpdate(mu.UnitId, mu, (key, old) => mu);
        }

        /// <summary>
        /// Enter player.
        /// </summary>
        public void EnterPlayer(PlayerUnit pu)
        {
            _playerUnitDict.AddOrUpdate(pu.UnitId, pu, (key, old) => pu);
        }

        /// <summary>
        /// Detect around mobs.
        /// </summary>
        public IEnumerable<MobUnit> DetectAroundMobs()
        {
            return _mobUnitDict.Values.Concat(_aroundWorldSections.SelectMany(d => d.GetMobUnits())).ToList();
        }

        /// <summary>
        /// Detect around players.
        /// </summary>
        public IEnumerable<PlayerUnit> DetectAroundPlayers()
        {
            return _playerUnitDict.Values.Concat(_aroundWorldSections.SelectMany(d => d.GetPlayerUnits())).ToList();
        }

        /// <summary>
        /// Detect player from around.
        /// </summary>
        /// <param name="srcUnitId">Srouce unit id</param>
        /// <param name="detectRangeSqr">Detect range square</param>
        public PlayerUnit DetectPlayerAround(long srcUnitId)
        {
            // Find this section.
            var srcPos = _mobUnitDict[srcUnitId].MovementHandler.NowPos();

            foreach (var pu in _playerUnitDict.Values)
            {
                if (GameData.MOB_DETECT_RANGE_SQUARE >= Vector2.DistanceSquared(srcPos, pu.MovementHandler.NowPos()))
                {
                    // 플레이어 감지.
                    return pu;
                }
            }

            // Find around sections.
            foreach (var ws in _aroundWorldSections)
            {
                foreach (var pu in ws.GetPlayerUnits())
                {
                    if (GameData.MOB_DETECT_RANGE_SQUARE >= Vector2.DistanceSquared(srcPos, pu.MovementHandler.NowPos()))
                    {
                        // 플레이어 감지.
                        return pu;
                    }
                }
            }

            return null;
        }
    }
}
