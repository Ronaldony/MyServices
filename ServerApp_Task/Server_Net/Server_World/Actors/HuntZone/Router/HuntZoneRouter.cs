using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.District;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;

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

        // NetPacket pipeline.
        private readonly ActionBlock<AroundPacketEvent> _netPacketPipeline;

        /// <summary>
        /// District proccessors.
        /// key: section group.
        /// value: District proccessor.
        /// </summary>
        private readonly Dictionary<SectionGroup, DistrictProcessor> _districtProcessors;

        /// <summary>
        /// 플레이어 섹션 정보.
        /// key: pid.
        /// value: Section group.
        /// </summary>
        private readonly Dictionary<string, SectionId> _playerSectionDict;

        /// <summary>
        /// World_SectionGroup.
        /// </summary>
        private readonly World_SectionGroup _worldSectionGroup;

        public HuntZoneRouter(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneRouter>>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // HuntZone 지역 정보.
            var confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

            // initialize sections.
            (_worldSectionGroup, _districtProcessors) = Initialize_WorldSectionGroup(serviceProvider, confArea, ct);

            // Player.
            _playerSectionDict = new Dictionary<string, SectionId>();

            // Pipeline.
            _gameEventPipeline = Initialize_GameEventPipeline(ct);
            _worldMsgPipeline = Initialize_WorldMsgPipeline(ct);
            _netPacketPipeline = Initialize_NetPacketPipeline(ct);
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

            return default;
        }

        /// <summary>
        /// Get around sections.
        /// </summary>
        public SectionGroup GetSectionGroup(SectionId section)
        {
            return _worldSectionGroup.SectionGroupDict[section];
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
        public bool RoutePacketAroundSections(AroundPacketEvent packetEvent)
        {
            return _netPacketPipeline.Post(packetEvent);
        }
    }
}
