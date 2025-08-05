using Server_Realm.Entities;

namespace Server_Realm.Zones.HuntZone
{
    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 이동.
        /// </summary>
        private void UpdateFrame_Movement(double deltaTime)
        {
            // Players.
            Parallel.ForEach(_worldGroup.PlayerUnitDict.Values, pu =>
            {
                // Sections.
                var oldSection = pu.MovementHandler.Section;

                pu.MovementHandler.Update(deltaTime);

                // 섹션 이동 검사.
                if (pu.MovementHandler.Section != oldSection)
                {
                    // 섹션 업데이트
                    pu.WorldSection.LeavePlayer(pu.UnitId);
                    pu.WorldSection = _worldGroup.WorldSectionDict[pu.MovementHandler.Section];
                    pu.WorldSection.EnterPlayer(pu);
                }
            });

            // Mobs.
            Parallel.ForEach(_worldGroup.MobUnitDict.Values, mu =>
            {
                var oldSection = mu.MovementHandler.Section;

                mu.MovementHandler.Update(deltaTime);

                // 섹션 이동 검사.
                if (mu.MovementHandler.Section != oldSection)
                {
                    // 섹션 업데이트
                    mu.WorldSection.LeaveMob(mu.UnitId);
                    mu.WorldSection = _worldGroup.WorldSectionDict[mu.MovementHandler.Section];
                    mu.WorldSection.EnterMob(mu);
                }
            });
        }
    }
}
