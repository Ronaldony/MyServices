namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Sections;

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
                var ws = _worldGroup.WorldSectionDict[section];

                // 투사체 업데이트.
                UpdateFrame_Projectile(deltaTime, ws);
            }
        }

        /// <summary>
        /// 프레임 업데이트 - 투사체 충돌 처리.
        /// </summary>
        private void UpdateFrame_Projectile(double deltaTime, WorldSection worldSection)
        {
            var aroundSections = GetAroundSections(worldSection.Section);

            // 플레이어 투사체와 몹 충돌 처리.
            foreach (var pu in worldSection.GetPlayerUnits())
            {
                // Update.
                pu.CombatHandler.Update(deltaTime);

                // 주변 섹션 대상으로 투사체 충돌.
                Parallel.ForEach(aroundSections, section =>
                {
                    pu.CombatHandler.CollideProjectiles(_worldGroup.WorldSectionDict[section].GetMobUnits());
                    pu.CombatHandler.CollideGrenade(_worldGroup.WorldSectionDict[section].GetMobUnits());
                });

                // Post.
                pu.CombatHandler.PostUpdate();
            }

            // 몹 투사체와 플레이어 충돌 처리.
            foreach (var mu in worldSection.GetMobUnits())
            {
                // Update.
                mu.CombatHandler.Update(deltaTime);

                // 주변 섹션 대상으로 투사체 충돌.
                Parallel.ForEach(aroundSections, section =>
                {
                    mu.CombatHandler.CollideProjectiles(_worldGroup.WorldSectionDict[section].GetPlayerUnits());
                    mu.CombatHandler.CollideGrenade(_worldGroup.WorldSectionDict[section].GetPlayerUnits());
                });

                // Post.
                mu.CombatHandler.PostUpdate();
            }
        }
    }
}