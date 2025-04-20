
using Dapper;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using Server_World.Core.Worlds;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 특정 섹션의 세션 가져오기.
        /// </summary>
        private List<GameSession> GetGameSessions((int, int) section)
        {
            if (false == _sectionPlayerDict.TryGetValue(section, out var playerDict))
            {
                return null;
            }

            if (false == playerDict.Any())
            {
                return null;
            }

            return playerDict.Values.Select(d => d.GameSession).AsList();
        }

        /// <summary>
        /// 특정 섹션의 플레이 정보 가져오기.
        /// </summary>
        private List<World_Player> GetWorldPlayers((int, int) section)
        {
            if (false == _sectionPlayerDict.TryGetValue(section, out var playerDict))
            {
                return null;
            }

            if (false == playerDict.Any())
            {
                return null;
            }

            return playerDict.Values.AsList();
        }

        /// <summary>
        /// 특정 섹션의 적 정보 가져오기.
        /// </summary>
        private List<DTO_Enemy> GetEnemies((int, int) section)
        {
            if (false == _sectionEnemyDict.TryGetValue(section, out var enemies))
            {
                return null;
            }

            if (false == enemies.Any())
            {
                return null;
            }

            return enemies.Values.AsList();
        }

        /// <summary>
        /// 섹션 세션 가져오기.
        /// </summary>
        private List<GameSession> GetAroundSessions((int, int) section)
        {
            var aroundGameSessions = new List<GameSession>();
            var aroundSections = GetAroundSections(section);

            foreach (var findSession in aroundSections)
            {
                var gameSessions = GetGameSessions(findSession);

                if (null == gameSessions)
                {
                    continue;
                }

                aroundGameSessions.AddRange(gameSessions);
            }

            return aroundGameSessions;
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
        private bool IsValidSection(int posX, int posY, out (int sectionX, int sectionY) outSection)
        {
            outSection = (posX / GameData.SECTION_WIDTH, posY / GameData.SECTION_HEIGHT);

            return _sectionDict.ContainsKey(outSection);
        }
    }
}
