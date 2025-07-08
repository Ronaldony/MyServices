using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_World.Actors.HuntZone.Processor;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// HuntZoneRouter.
    /// </summary>
    public partial class HuntZoneRouter
    {
        private readonly ILogger<HuntZoneRouter> _logger;

        // Packet pipeline.
        private readonly ActionBlock<GameEvent> _gameEventPipeline;

        // Message pipeline.
        private readonly ActionBlock<WorldMsg> _worldMsgPipeline;

        /// <summary>
        /// Game proccessors.
        /// key: section group.
        /// value: Game proccessor.
        /// </summary>
        private readonly Dictionary<SectionGroup, GameProcessor> _gameProccessors;

        /// <summary>
        /// 유저 섹션 정보.
        /// key: pid.
        /// value: Section group.
        /// </summary>
        private readonly ConcurrentDictionary<string, SectionGroup> _playerSectionGroupDict;

        /// <summary>
        /// 근처 섹션.
        /// key: SectionId.
        /// value: Around sections.
        /// </summary>
        private readonly Dictionary<SectionId, HashSet<SectionId>> _aroundSectionDict;

        /// <summary>
        /// 섹션 그룹.
        /// key: SectionId.
        /// value: SectionGroup.
        /// </summary>
        private readonly Dictionary<SectionId, SectionGroup> _sectionGroupDict;

        public HuntZoneRouter(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<HuntZoneRouter>>();

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // HuntZone 지역 정보.
            var confArea = tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

            // initialize sections.
            _aroundSectionDict = Initialize_AroundSection(confArea);
            (_sectionGroupDict, _gameProccessors) = Initialize_SectionGroup(serviceProvider, confArea, ct);

            // Pipeline.
            _gameEventPipeline = initialize_GameEventPipeline(ct);
            _worldMsgPipeline = initialize_WorldMsgPipeline(ct);
        }

        /// <summary>
        /// Get around sections.
        /// </summary>
        public HashSet<SectionId> GetAroundSections(SectionId section)
        {
            if (_aroundSectionDict.ContainsKey(section))
            {
                return _aroundSectionDict[section];
            }

            return null;
        }

        /// <summary>
        /// Get around sections.
        /// </summary>
        public SectionGroup GetSectionGroup(SectionId section)
        {
            return _sectionGroupDict[section];
        }

        /// <summary>
        /// Validate section.
        /// </summary>
        public bool IsValidSection(SectionId section)
        {
            if (_aroundSectionDict.ContainsKey(section))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Post world packet.
        /// </summary>
        public bool PostGameEvent(GameEvent worldPacket)
        {
            return _gameEventPipeline.Post(worldPacket);
        }

        /// <summary>
        /// Post world message.
        /// </summary>
        public bool PostWorldMsg(WorldMsg worldMsg)
        {
            return _worldMsgPipeline.Post(worldMsg);
        }
    }
}
