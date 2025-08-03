namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using Server_Realm.Entities;
    using Server_Realm.Units.Enemy;
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Initialize world group.
        /// </summary>
        private World_Group Initialize_WorldGroup()
        {
            var aroundSessionDict = new Dictionary<SectionId, HashSet<SectionId>>();
            var worldSectionDict = new Dictionary<SectionId, World_Section>();

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
                    var section = new SectionId(sectionX, sectionY);

                    aroundSessionDict.Add(section, new HashSet<SectionId>());
                    worldSectionDict.Add(section, new World_Section(section));
                }
            }

            // 근처 섹션 계산.
            foreach (var (srcSection, aroundSections) in aroundSessionDict)
            {
                // 근처 섹션 추가.
                var calcAroundSections = Calc_AroundSections(sectionMinX, sectionMaxX, sectionMinY, sectionMaxY, srcSection.X, srcSection.Y);

                foreach (var section in calcAroundSections)
                {
                    // 인접한 Section 추가.
                    aroundSections.Add(section);

                    // 인접한 WorldSection 추가.
                    if (srcSection != section)
                    {
                        worldSectionDict[srcSection].AddAroundWorldSection(worldSectionDict[section]);
                    }
                }
            }

            return new World_Group
            {
                PlayerUnitDict = new Dictionary<long, PlayerUnit>(),
                MobUnitDict = new Dictionary<long, MobUnit>(),
                AroundSectionDict = aroundSessionDict,
                WorldSectionDict = worldSectionDict
            };
        }

        /// <summary>
        /// Calculate around sections.
        /// </summary>
        private List<SectionId> Calc_AroundSections(int sectionMinX, int sectionMaxX, int sectionMinY, int sectionMaxY, int sectionX, int sectionY)
        {
            var aroundSections = new List<SectionId>();

            // 현재 섹션 추가.
            aroundSections.Add(new SectionId(sectionX, sectionY));

            // Top.
            var topSection = new SectionId(sectionX, sectionY + 1);
            if (topSection.Y <= sectionMaxY)
            {
                // 최대 Y 섹션 이하인 경우.
                aroundSections.Add(topSection);
            }

            // Down.
            var downSection = new SectionId(sectionX, sectionY - 1);
            if (downSection.Y >= sectionMinY)
            {
                // 최소 Y 섹션 이상인 경우.
                aroundSections.Add(downSection);
            }

            // Left.
            var leftSection = new SectionId(sectionX - 1, sectionY);
            if (leftSection.X >= sectionMinX)
            {
                // 최소 X 섹션 이상인 경우.
                aroundSections.Add(leftSection);
            }

            // Right.
            var rightSection = new SectionId(sectionX + 1, sectionY);
            if (rightSection.X <= sectionMaxX)
            {
                // 최대 X 섹션 이하인 경우.
                aroundSections.Add(rightSection);
            }

            // LeftTop.
            var leftTopSection = new SectionId(sectionX - 1, sectionY + 1);
            if (leftTopSection.X >= sectionMinX && leftTopSection.Y <= sectionMaxY)
            {
                // 최소 X 섹션 이상, 최대 Y 섹션 이하인 경우.
                aroundSections.Add(leftTopSection);
            }

            // LeftDown.
            var leftDownSection = new SectionId(sectionX - 1, sectionY - 1);
            if (leftDownSection.X >= sectionMinX && leftDownSection.Y >= sectionMinY)
            {
                // 최소 X 섹션 이상, 최소 Y 섹션 이상인 경우.
                aroundSections.Add(leftDownSection);
            }

            // RightTop.
            var rightTopSection = new SectionId(sectionX + 1, sectionY + 1);
            if (rightTopSection.X <= sectionMaxX && rightTopSection.Y <= sectionMaxY)
            {
                // 최대 X 섹션 이하, 최대 Y 섹션 이하인 경우.
                aroundSections.Add(rightTopSection);
            }

            // RightDown.
            var rightDownSection = new SectionId(sectionX + 1, sectionY - 1);
            if (rightDownSection.X <= sectionMaxX && rightDownSection.Y >= sectionMinY)
            {
                // 최대 X 섹션 이하, 최소 Y 섹션 이상인 경우.
                aroundSections.Add(rightDownSection);
            }

            return aroundSections;
        }
    }
}
