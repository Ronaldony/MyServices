namespace Server_Realm.Zones.HuntZone.District
{
    using Server_Realm.Worlds.Update;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 유닛 업데이트.
        /// </summary>
        private void UpdateFrame_UpdateFrameUnits(double deltaTime, WorldUpdate_Frame updateFrame)
        {
            // 업데이트 프레임 데이터.
            var moveSectionUnit = updateFrame.MoveSectionUnits;
            var moveDistrictUnit = updateFrame.MoveDistrictUnits;

            #region update players
            // 유닛 업데이트 - 플레이어.
            foreach (var pair in _worldGroup.SectionPlayerDict)
            {
                var section = pair.Key;
                
                foreach (var pu in pair.Value.Values)
                {
                    pu.Update(deltaTime);

                    var moveSection = pu.MovementHandler.NowSection();

                    if (moveSection != section)
                    {
                        _worldGroup.SectionPlayerDict[section].TryRemove(pu.Pid, out var _);

                        // 섹션 이동과 구역 이동 처리.
                        if (IsManagedSection(moveSection))
                        {
                            // 섹션 이동.
                            _worldGroup.SectionPlayerDict[moveSection].AddOrUpdate(pu.Pid, pu, (_, _) => pu);

                            moveSectionUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<WorldUpdate_SectionUnit>())
                            .PlayInfoDict.AddOrUpdate(pu.UnitId, pu.PlayInfo, (_, _) => pu.PlayInfo);
                        }
                        else
                        {
                            // 구역 이동.
                            moveDistrictUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<WorldUpdate_DistrictUnit>())
                            .PlayerUnits.Add(pu);

                            // 구역에서 플레이어 제거.
                            _worldGroup.PlayerDict.Remove(pu.Pid);
                        }
                    }
                }
            }
            #endregion

            #region update mobs
            // 유닛 업데이트 - 몹.
            foreach (var pair in _worldGroup.SectionMobDict)
            {
                var section = pair.Key;

                foreach (var mu in pair.Value.Values)
                {
                    mu.Update(deltaTime);

                    // 적 위치 계산.
                    var moveSection = mu.MovementHandler.NowSection();

                    if (moveSection != section)
                    {
                        _worldGroup.SectionMobDict[section].TryRemove(mu.UnitId, out var _);

                        // 섹션 이동과 구역 이동 처리.
                        if (IsManagedSection(moveSection))
                        {
                            // 섹션 이동.
                            _worldGroup.SectionMobDict[moveSection].AddOrUpdate(mu.UnitId, mu, (_, _) => mu);

                            // 섹션 이동.
                            moveSectionUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<WorldUpdate_SectionUnit>())
                            .MobDict.AddOrUpdate(mu.UnitId, mu.Mob, (_, _) => mu.Mob);
                        }
                        else
                        {
                            // 구역에서 플레이어 제거.
                            _worldGroup.MobDict.Remove(mu.UnitId);

                            // 구역 이동.
                            moveDistrictUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<WorldUpdate_DistrictUnit>())
                            .MobUnits.Add(mu);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
