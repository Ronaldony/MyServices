
using Dapper;
using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using Server_World.Core.Datas;
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

        /// <summary>
        /// Initialize seciton informations.
        /// </summary>
        private (
            Dictionary<(int, int), ConcurrentDictionary<string, World_Player>>,
            Dictionary<(int, int), ConcurrentDictionary<ulong, DTO_Enemy>>,
            Dictionary<(int, int), List<(int, int)>>,
            Dictionary<(int sectionX, int sectionY), Section>
            ) 
            Initialize_Sections()
        {
            var sectionPlayerDict = new Dictionary<(int, int), ConcurrentDictionary<string, World_Player>>();
            var sectionEnemyDict = new Dictionary<(int, int), ConcurrentDictionary<ulong, DTO_Enemy>>();
            var aroundSessionDict = new Dictionary<(int sectionX, int sectionY), List<(int, int)>>();
            var sectionDict = new Dictionary<(int sectionX, int sectionY), Section>();

            // 지역 정보 불러오기.
            var confArea = _tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

            // 지역 좌표를 계산하여 섹션 분리.

            // 가로, 세로 섹션의 최소, 최댓 값을 사용하여 섹션 분리.
            var sectionMinX = confArea.MinX / GameData.SECTION_WIDTH;
            var sectionMaxX = confArea.MaxX / GameData.SECTION_WIDTH;

            var sectionMinY = confArea.MinY / GameData.SECTION_HEIGHT;
            var sectionMaxY = confArea.MaxY / GameData.SECTION_HEIGHT;

            // 가로, 세로 섹션 저장.
            for (var sectionX = sectionMinX; sectionX <= sectionMaxX; ++sectionX)
            {
                for (var sectionY = sectionMinY; sectionY <= sectionMaxY; ++sectionY)
                {
                    var section = (sectionX, sectionY);

                    sectionPlayerDict.Add(section, new ConcurrentDictionary<string, World_Player>());
                    sectionEnemyDict.Add(section, new ConcurrentDictionary<ulong, DTO_Enemy>());
                    aroundSessionDict.Add(section, new List<(int, int)>());
                    sectionDict.Add(section, new Section
                    {
                        X = sectionX,
                        Y = sectionY,
                    });
                }
            }

            // 근처 섹션 계산.
            foreach (var pair in aroundSessionDict)
            {
                var aroundSections = pair.Value;
                var sectionX = pair.Key.sectionX;
                var sectionY = pair.Key.sectionY;

                var calcAroundSections = Calc_AroundSections(sectionMinX, sectionMaxX, sectionMinY, sectionMaxY, sectionX, sectionY);

                aroundSections.AddRange(calcAroundSections);
            }

            return (sectionPlayerDict, sectionEnemyDict, aroundSessionDict, sectionDict);
        }

        /// <summary>
        /// Calculate around sections.
        /// </summary>
        private List<(int, int)> Calc_AroundSections(int sectionMinX, int sectionMaxX, int sectionMinY, int sectionMaxY, int sectionX, int sectionY)
        {
            var aroundSections = new List<(int, int)>();

            // 현재 섹션 추가.
            aroundSections.Add((sectionX, sectionY));

            // Top.
            var topSection = (sectionX, sectionY + 1);
            if (topSection.Item2 <= sectionMaxY)
            {
                // 최대 Y 섹션 이하인 경우.
                aroundSections.Add(topSection);
            }

            // Down.
            var downSection = (sectionX, sectionY - 1);
            if (downSection.Item2 >= sectionMinY)
            {
                // 최소 Y 섹션 이상인 경우.
                aroundSections.Add(downSection);
            }

            // Left.
            var leftSection = (sectionX - 1, sectionY);
            if (leftSection.Item1 >= sectionMinX)
            {
                // 최소 X 섹션 이상인 경우.
                aroundSections.Add(leftSection);
            }

            // Right.
            var rightSection = (sectionX + 1, sectionY);
            if (rightSection.Item1 <= sectionMaxX)
            {
                // 최대 X 섹션 이하인 경우.
                aroundSections.Add(rightSection);
            }

            // LeftTop.
            var leftTopSection = (sectionX - 1, sectionY + 1);
            if (leftTopSection.Item1 >= sectionMinX && leftTopSection.Item2 <= sectionMaxY)
            {
                // 최소 X 섹션 이상, 최대 Y 섹션 이하인 경우.
                aroundSections.Add(leftTopSection);
            }

            // LeftDown.
            var leftDownSection = (sectionX - 1, sectionY - 1);
            if (leftDownSection.Item1 >= sectionMinX && leftDownSection.Item2 >= sectionMinY)
            {
                // 최소 X 섹션 이상, 최소 Y 섹션 이상인 경우.
                aroundSections.Add(leftDownSection);
            }

            // RightTop.
            var rightTopSection = (sectionX + 1, sectionY + 1);
            if (rightTopSection.Item1 <= sectionMaxX && rightTopSection.Item2 <= sectionMaxY)
            {
                // 최대 X 섹션 이하, 최대 Y 섹션 이하인 경우.
                aroundSections.Add(rightTopSection);
            }

            // RightDown.
            var rightDownSection = (sectionX + 1, sectionY - 1);
            if (rightDownSection.Item1 <= sectionMaxX && rightDownSection.Item2 >= sectionMinY)
            {
                // 최대 X 섹션 이하, 최소 Y 섹션 이상인 경우.
                aroundSections.Add(rightDownSection);
            }

            return aroundSections;
        }
    }
}
