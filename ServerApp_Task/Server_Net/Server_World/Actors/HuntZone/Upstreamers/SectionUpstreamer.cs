using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;

namespace Server_World.Actors.HuntZone.Processors
{
    using GameCore.Interfaces;
    using Server_World.Core.Datas;

    /// <summary>
    /// SectionUpstreamer.
    /// </summary>
    public partial class SectionUpstreamer
    {
        private readonly ILogger<SectionUpstreamer> _logger;

        private readonly ITDBService _tdbService;

        /// <summary>
        /// 유저 섹션 정보.
        /// key: section.
        /// value: player position. (key: pid, value: position)
        /// </summary>
        private readonly Dictionary<SectionId, ConcurrentDictionary<string, Vector2>> _sections;

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

        public SectionUpstreamer(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<SectionUpstreamer>>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();

            // initialize sections.
            _aroundSectionDict = Initialize_AroundSection();
            _sectionGroupDict = Initialize_SectionGroup();
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
    }
}
