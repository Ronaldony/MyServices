using System.Collections.Concurrent;

namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Update;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 유닛 업데이트.
        /// </summary>
        private void UpdateFrame_UpdateUnits(double deltaTime, ConcurrentDictionary<SectionId, WorldUpdate_SectionUnit> moveSectionUnit)
        {
            // 업데이트 프레임 데이터.
            #region update players
            var taskUpdatePlayer = Task.Run(() =>
            {
                // 유닛 업데이트 - 플레이어.
                Parallel.ForEach(_worldGroup.PlayerDict.Values, pu =>
                {
                    var oldSection = pu.MovementHandler.Section;

                    pu.Update(deltaTime);

                    var newSection = pu.MovementHandler.Section;

                    if (newSection != oldSection)
                    {
                        // 섹션 업데이트.
                        _worldGroup.WorldSectionDict[oldSection].LeavePlayer(pu.Pid);
                        _worldGroup.WorldSectionDict[newSection].EnterPlayer(pu);

                        // 섹션 이동 플레이어 저장.
                        moveSectionUnit
                        .GetOrAdd(newSection, _memoryPoolService.Rent<WorldUpdate_SectionUnit>())
                        .PlayInfoDict.AddOrUpdate(pu.UnitId, pu.PlayInfo, (_, _) => pu.PlayInfo);
                    }
                });
            });
            #endregion

            #region update mobs
            var taskUpdateMob = Task.Run(() =>
            {
                Parallel.ForEach(_worldGroup.MobDict.Values, mu =>
                {
                    var oldSection = mu.MovementHandler.Section;

                    mu.Update(deltaTime);

                    // 적 위치 계산.
                    var moveSection = mu.MovementHandler.Section;

                    if (moveSection != oldSection)
                    {
                        // 섹션 업데이트.
                        _worldGroup.WorldSectionDict[oldSection].LeaveMob(mu.UnitId);
                        _worldGroup.WorldSectionDict[moveSection].EnterMob(mu);

                        // 섹션 이동 플레이어 저장.
                        moveSectionUnit
                        .GetOrAdd(moveSection, _memoryPoolService.Rent<WorldUpdate_SectionUnit>())
                        .MobDict.AddOrUpdate(mu.UnitId, mu.Mob, (_, _) => mu.Mob);
                    }
                });
            });
            #endregion

            // Wait tasks.
            Task.WhenAll(taskUpdatePlayer, taskUpdateMob).Wait();
        }
    }
}
