using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// Initialize seciton informations.
        /// </summary>
        private (
            Dictionary<(int, int), List<(int, int)>>,
            Dictionary<(int sectionX, int sectionY), Section>
            )
            Initialize_Sections()
        {
            var aroundSessionDict = new Dictionary<(int sectionX, int sectionY), List<(int, int)>>();
            var sectionDict = new Dictionary<(int sectionX, int sectionY), Section>();

            // 지역 정보 불러오기.
            var confArea = _tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

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

                    aroundSessionDict.Add(section, new List<(int, int)>());
                    sectionDict.Add(section, new Section(sectionX, sectionY));
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

            return (aroundSessionDict, sectionDict);
        }

        /// <summary>
        /// Initialize world group.
        /// </summary>
        private World_Group Initialize_WorldGroup()
        {
            var sectionPlayerDict = new Dictionary<(int, int), ConcurrentDictionary<string, World_Player>>();
            var sectionEnemyDict = new Dictionary<(int, int), ConcurrentDictionary<ulong, DTO_Enemy>>();

            // 지역 정보 불러오기.
            var confArea = _tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

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
                }
            }

            return new World_Group
            {
                EnemyDict = new Dictionary<ulong, DTO_Enemy>(),
                PlayerDict = new Dictionary<string, World_Player>(),
                SectionPlayerDict = sectionPlayerDict,
                SectionEnemyDict = sectionEnemyDict,
            };
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
