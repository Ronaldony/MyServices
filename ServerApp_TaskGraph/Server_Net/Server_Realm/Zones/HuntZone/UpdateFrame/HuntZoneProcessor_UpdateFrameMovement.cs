namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 이동.
        /// </summary>
        private void UpdateFrame_Movement(double deltaTime, World_Section worldSection)
        {
            #region update players
            // 플레이어 업데이트.
            foreach (var pu in worldSection.GetPlayerUnits())
            {
                // 이동 업데이트
                pu.MovementHandler.Update(deltaTime);

                var newSection = pu.MovementHandler.Section;

                if (newSection != worldSection.Section)
                {
                    // 섹션 업데이트.
                    worldSection.LeavePlayer(pu.UnitId);
                    _worldGroup.WorldSectionDict[newSection].EnterPlayer(pu);
                }
            }
            #endregion

            #region update mobs
            // 몹 업데이트.
            foreach (var mu in worldSection.GetMobUnits())
            {
                mu.MovementHandler.Update(deltaTime);

                // 적 위치 계산.
                var moveSection = mu.MovementHandler.Section;

                if (moveSection != worldSection.Section)
                {
                    // 섹션 업데이트.
                    worldSection.LeaveMob(mu.UnitId);
                    _worldGroup.WorldSectionDict[moveSection].EnterMob(mu);
                }
            }
            #endregion
        }
    }
}
