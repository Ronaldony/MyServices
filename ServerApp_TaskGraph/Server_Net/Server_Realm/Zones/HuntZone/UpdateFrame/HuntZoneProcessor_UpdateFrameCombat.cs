namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 전투.
        /// </summary>
        private void UpdateFrame_Combat(double deltaTime, List<SectionId> sections)
        {
            foreach (var section in sections)
            {
                // 유닛 업데이트.
                var worldSection = _worldGroup.WorldSectionDict[section];

                // 투사체 업데이트.
                UpdateFrame_Projectile(deltaTime, worldSection);
            }
        }

        /// <summary>
        /// 프레임 업데이트 - 투사체 충돌 처리.
        /// </summary>
        private void UpdateFrame_Projectile(double deltaTime, World_Section worldSection)
        {
            var aroundSections = GetAroundSections(worldSection.Section);

            // 플레이어 투사체와 몹 충돌 처리.
            foreach (var pu in worldSection.GetPlayerUnits())
            {
                pu.CombatHandler.Update(deltaTime);

                // 주변 섹션 대상으로 투사체 충돌.
                Parallel.ForEach(aroundSections, section =>
                {
                    pu.CombatHandler.CollideProjectiles(_worldGroup.WorldSectionDict[section].GetMobUnits());
                });
            }

            // 몹 투사체와 플레이어 충돌 처리.
            foreach (var mu in worldSection.GetMobUnits())
            {
                mu.CombatHandler.Update(deltaTime);

                // 주변 섹션 대상으로 투사체 충돌.
                Parallel.ForEach(aroundSections, section =>
                {
                    mu.CombatHandler.CollideProjectiles(_worldGroup.WorldSectionDict[section].GetPlayerUnits());
                });
            }
        }
    }
}