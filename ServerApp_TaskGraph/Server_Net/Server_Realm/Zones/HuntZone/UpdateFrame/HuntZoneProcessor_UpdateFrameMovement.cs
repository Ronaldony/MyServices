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
        private async Task UpdateFrame_MovementAsync(double deltaTime)
        {
            // Players.
            var playerTask = Task.Run(() =>
            {
                Parallel.ForEach(_worldGroup.PlayerUnitDict.Values, pu =>
                {
                    var oldSection = pu.MovementHandler.Section;

                    pu.MovementHandler.Update(deltaTime);

                    var newSection = pu.MovementHandler.Section;

                    // 섹션 이동 검사.
                    if (newSection != oldSection)
                    {
                        // 섹션 업데이트
                        pu.WorldSection.LeavePlayer(pu.UnitId);
                        pu.WorldSection = _worldGroup.WorldSectionDict[newSection];
                        pu.WorldSection.EnterPlayer(pu);
                    }
                });
            });

            // Mobs.
            var mobTask = Task.Run(() =>
            {
                Parallel.ForEach(_worldGroup.MobUnitDict.Values, mu =>
                {
                    var oldSection = mu.MovementHandler.Section;

                    mu.MovementHandler.Update(deltaTime);

                    var newSection = mu.MovementHandler.Section;

                    // 섹션 이동 검사.
                    if (newSection != oldSection)
                    {
                        // 섹션 업데이트
                        mu.WorldSection.LeaveMob(mu.UnitId);
                        mu.WorldSection = _worldGroup.WorldSectionDict[newSection];
                        mu.WorldSection.EnterMob(mu);
                    }
                });
            });

            // wait for movements.
            await Task.WhenAll(playerTask, mobTask);
        }
    }
}
