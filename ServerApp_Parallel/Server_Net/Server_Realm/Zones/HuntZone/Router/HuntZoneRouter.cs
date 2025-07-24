using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Server_Realm.Zones.HuntZone.Router
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.GameEvents;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones.HuntZone.District;

    /// <summary>
    /// HuntZoneRouter.
    /// </summary>
    public partial class HuntZoneRouter
    {
        private readonly ILogger<HuntZoneRouter> _logger;

        // GameEvent pipeline.
        private readonly ActionBlock<GameEvent> _gameEventPipeline;

        // WorldMsg pipeline.
        private readonly ActionBlock<WorldMsg> _worldMsgPipeline;

        /// <summary>
        /// World_SectionGroup.
        /// </summary>
        private readonly World_SectionGroup _worldSectionGroup;

        /// <summary>
        /// District proccessors.
        /// key: section group.
        /// value: District proccessor.
        /// </summary>
        private readonly Dictionary<District, DistrictProcessor> _districtProcessors;

        /// <summary>
        /// 플레이어 섹션 정보.
        /// key: pid.
        /// value: Section group.
        /// </summary>
        private readonly ConcurrentDictionary<string, District> _playerDistrictDict;

        public HuntZoneRouter(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneRouter>>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // HuntZone 지역 정보.
            var confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

            // initialize sections.
            (_worldSectionGroup, _districtProcessors) = Initialize_WorldSectionGroup(serviceProvider, confArea, ct);

            // Player.
            _playerDistrictDict = new ConcurrentDictionary<string, District>();

            // Pipeline.
            _gameEventPipeline = Initialize_GameEventPipeline(ct);
            _worldMsgPipeline = Initialize_WorldMsgPipeline(ct);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            // Initialize DistrictProcessor.
            foreach (var (district, processor) in _districtProcessors)
            {
                processor.Initialize();
            }

            _logger.LogInformation($"HuntZoneRouter initialized.");
        }

        /// <summary>
        /// Get around sections.
        /// </summary>
        public HashSet<SectionId> GetAroundSections(SectionId section)
        {
            if (_worldSectionGroup.AroundSectionDict.ContainsKey(section))
            {
                return _worldSectionGroup.AroundSectionDict[section];
            }

            return null;
        }

        /// <summary>
        /// Get around district.
        /// </summary>
        public HashSet<District> GetAroundDistrict(SectionId section)
        {
            if (_worldSectionGroup.AroundDistrictDict.ContainsKey(section))
            {
                return _worldSectionGroup.AroundDistrictDict[section];
            }

            return null;
        }

        /// <summary>
        /// Get district.
        /// </summary>
        private District GetDistrict(SectionId section)
        {
            return _worldSectionGroup.DistrictDict[section];
        }

        /// <summary>
        /// Validate section.
        /// </summary>
        public bool IsValidSection(SectionId section)
        {
            if (_worldSectionGroup.AroundSectionDict.ContainsKey(section))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Post world packet.
        /// </summary>
        public bool RouteGameEvent(GameEvent worldPacket)
        {
            return _gameEventPipeline.Post(worldPacket);
        }

        /// <summary>
        /// Post world message.
        /// </summary>
        public bool RouteWorldMsg(WorldMsg worldMsg)
        {
            return _worldMsgPipeline.Post(worldMsg);
        }

        /// <summary>
        /// Send packet around section.
        /// </summary>
        public void BroadcastPacketAround(BroadcastPacketEvent packetEvent)
        {
            var aroundDistrict = GetAroundDistrict(packetEvent.RouteSection);

            // 주변 구역에 전파.
            foreach (var district in aroundDistrict)
            {
                _districtProcessors[district].WriteGameEvent(packetEvent);
            }
        }
    }
}
