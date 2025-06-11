using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using Server_World.Core.Worlds;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// Get section.
        /// </summary>
        private (int sectionX, int sectionY) GetSection(Vector2 position)
        {
            return ((int)position.X / GameData.SECTION_WIDTH, (int)position.Y / GameData.SECTION_HEIGHT);
        }

        /// <summary>
        /// 특정 섹션의 플레이 정보 가져오기.
        /// </summary>
        private IEnumerable<World_Player> GetSectionPlayers((int, int) section)
        {
            if (false == _sectionPlayerDict.TryGetValue(section, out var playerDict))
            {
                return null;
            }

            return playerDict.Values;
        }

        /// <summary>
        /// 특정 섹션의 적 정보 가져오기.
        /// </summary>
        private IEnumerable<DTO_Enemy> GetSectionEnemies((int, int) section)
        {
            if (false == _sectionEnemyDict.TryGetValue(section, out var enemies))
            {
                return null;
            }

            return enemies.Values;
        }

        /// <summary>
        /// Get players around sections.
        /// </summary>
        private IEnumerable<World_Player> GetAroundPlayers((int, int) section)
        {
            var aroundPlayers = new List<World_Player>();
            var aroundSections = GetAroundSections(section);

            foreach (var findSection in aroundSections)
            {
                var sectionPlayers = GetSectionPlayers(findSection);

                if (false == sectionPlayers.Any())
                {
                    continue;
                }

                aroundPlayers.AddRange(sectionPlayers);
            }

            return aroundPlayers;
        }

        /// <summary>
        /// 근처 섹션 가져오기.
        /// </summary>
        private List<(int sectionX, int sectionY)> GetAroundSections((int, int) section)
        {
            if (false == _aroundSectionDict.TryGetValue(section, out var aroundSections))
            {
                return null;
            }

            return aroundSections;
        }

        /// <summary>
        /// Get section or default.
        /// </summary>
        private bool IsValidSection((int sectionX, int sectionY) section)
        {
            return _sectionDict.ContainsKey(section);
        }
    }
}
