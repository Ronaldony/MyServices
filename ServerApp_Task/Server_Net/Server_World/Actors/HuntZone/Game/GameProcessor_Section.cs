
using System.Numerics;

namespace Server_World.Actors.HuntZone.Game
{
    using GameCore;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds;

    /// <summary>
    /// GameProcessor.
    /// </summary>
    public partial class GameProcessor
    {
        /// <summary>
        /// Get section.
        /// </summary>
        private SectionId GetSection(Vector2 position)
        {
            return new SectionId((int)position.X / GameData.SECTION_WIDTH, (int)position.Y / GameData.SECTION_HEIGHT);
        }

        /// <summary>
        /// 특정 섹션의 플레이 정보 가져오기.
        /// </summary>
        private ICollection<World_Player> GetSectionPlayers(SectionId section)
        {
            if (false == _worldGroup.SectionPlayerDict.TryGetValue(section, out var playerDict))
            {
                return new List<World_Player>();
            }

            return playerDict.Values;
        }

        /// <summary>
        /// 특정 섹션의 적 정보 가져오기.
        /// </summary>
        private ICollection<World_Enemy> GetSectionEnemies(SectionId section)
        {
            if (false == _worldGroup.SectionEnemyDict.TryGetValue(section, out var enemies))
            {
                return new List<World_Enemy>();
            }

            return enemies.Values;
        }

        /// <summary>
        /// Get players around sections.
        /// </summary>
        private ICollection<World_Player> GetAroundPlayers(SectionId section)
        {
            var aroundPlayers = new List<World_Player>();
            var aroundSections = GetAroundSections(section);

            foreach (var findSection in aroundSections)
            {
                aroundPlayers.AddRange(GetSectionPlayers(findSection));
            }

            return aroundPlayers;
        }

        /// <summary>
        /// 근처 섹션 가져오기.
        /// </summary>
        private HashSet<SectionId> GetAroundSections(SectionId section)
        {
            if (false == _worldGroup.AroundSectionDict.TryGetValue(section, out var aroundSections))
            {
                return new HashSet<SectionId>();
            }

            return aroundSections;
        }

        /// <summary>
        /// Get section or default.ss
        /// </summary>
        private bool IsValidSection(SectionId section)
        {
            return _worldGroup.AroundSectionDict.ContainsKey(section);
        }
    }
}
