namespace Server_World.Actors.HuntZone.Router
{
    using GameCore;
    using Server_World.Actors.HuntZone.District;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds;

    /// <summary>
    /// HuntZoneRouter.
    /// </summary>
    public partial class HuntZoneRouter
    {
        /// <summary>
        /// Initialize WorldSectionGroup.
        /// </summary>
        private (World_SectionGroup, Dictionary<District, DistrictProcessor>)
            Initialize_WorldSectionGroup(IServiceProvider serviceProvider, CONF_Area confArea, CancellationToken ct)
        {
            // 가로, 세로 섹션의 최소, 최댓 값을 사용하여 섹션 분리.
            var sectionMinX = confArea.MinX / GameData.SECTION_WIDTH;
            var sectionMaxX = confArea.MaxX / GameData.SECTION_WIDTH;

            var sectionMinY = confArea.MinY / GameData.SECTION_HEIGHT;
            var sectionMaxY = confArea.MaxY / GameData.SECTION_HEIGHT;

            /////////////////////////////////////////////////////////////////////////////
            /// 주변 섹션 그루핑.
            var aroundSectionDict = new Dictionary<SectionId, HashSet<SectionId>>();

            // 가로, 세로 섹션 저장.
            for (var sectionX = sectionMinX; sectionX <= sectionMaxX; ++sectionX)
            {
                for (var sectionY = sectionMinY; sectionY <= sectionMaxY; ++sectionY)
                {
                    var section = new SectionId(sectionX, sectionY);

                    aroundSectionDict.Add(section, new HashSet<SectionId>());
                }
            }

            // 주변 섹션 계산.
            foreach (var pair in aroundSectionDict)
            {
                var aroundSections = pair.Value;
                var sectionX = pair.Key.X;
                var sectionY = pair.Key.Y;

                // 주변 섹션 추가.
                var calcAroundSections = Calc_AroundSections(sectionMinX, sectionMaxX, sectionMinY, sectionMaxY, sectionX, sectionY);
                foreach (var section in calcAroundSections)
                {
                    aroundSections.Add(section);
                }
            }

            /////////////////////////////////////////////////////////////////////////////
            /// 구역.
            var districtDict = new Dictionary<SectionId, District>();
            var gameProcessorDict = new Dictionary<District, DistrictProcessor>();

            // Section group increment.
            int districtInc = 1;

            // 가로, 세로 섹션 저장.
            for (var sectionX = sectionMinX; sectionX <= sectionMaxX; sectionX += 2)
            {
                for (var sectionY = sectionMinY; sectionY <= sectionMaxY; sectionY += 2)
                {
                    var district = new District(districtInc);
                    var aroundSections = new HashSet<SectionId>();

                    // first.
                    var sectionId = new SectionId(sectionX, sectionY);
                    districtDict.Add(sectionId, district);
                    aroundSections.Add(sectionId);

                    // second.
                    if (sectionX + 1 <= sectionMaxX)
                    {
                        sectionId = new SectionId(sectionX + 1, sectionY);
                        districtDict.Add(sectionId, district);
                        aroundSections.Add(sectionId);
                    }

                    // third.
                    if (sectionY + 1 <= sectionMaxY)
                    {
                        sectionId = new SectionId(sectionX, sectionY + 1);
                        districtDict.Add(sectionId, district);
                        aroundSections.Add(sectionId);
                    }

                    // forth.
                    if (sectionX + 1 <= sectionMaxX && sectionY + 1 <= sectionMaxY)
                    {
                        sectionId = new SectionId(sectionX + 1, sectionY + 1);
                        districtDict.Add(sectionId, district);
                        aroundSections.Add(sectionId);
                    }

                    gameProcessorDict.Add(district, new DistrictProcessor(serviceProvider, this, aroundSections, district, ct));
                    districtInc++;
                }
            }

            /////////////////////////////////////////////////////////////////////////////
            /// 주변 구역.
            var aroundDistrictDict = new Dictionary<SectionId, HashSet<District>>();

            foreach (var pair in aroundSectionDict)
            {
                var section = pair.Key;
                var district = districtDict[section];

                aroundDistrictDict.Add(pair.Key, new HashSet<District>());

                // 주변 구역 저장.
                foreach (var aroundSection in pair.Value)
                {
                    // 현재 구역과 겹치는지 검사.
                    if (district != districtDict[aroundSection]) 
                    {
                        aroundDistrictDict[section].Add(districtDict[aroundSection]);
                    }
                }
            }

            // WorldSectionGroup.
            var wroldSectionGroup = new World_SectionGroup
            {
                AroundSectionDict = aroundSectionDict,
                AroundDistrictDict = aroundDistrictDict,
                DistrictDict = districtDict,
            };

            return (_worldSectionGroup, gameProcessorDict);
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
