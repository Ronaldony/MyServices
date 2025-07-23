using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;

namespace Server_World.Zones.HuntZone.District
{
    using GameCore;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Section;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 이동 처리.
        /// </summary>
        private void UpdateFrame_Move(double deltaTime, World_UpdateFrame updateFrame)
        {
            // 업데이트 프레임 데이터.
            var moveSectionUnit = updateFrame.MoveSectionUnit;
            var moveDistrictUnit = updateFrame.MoveDistrictUnit;

            // 플레이어가 존재하는 섹션의 플레이어, 적 이동 처리.
            Parallel.ForEach(_worldGroup.ManagedSections, section =>
            {
                // 섹션 유닛.
                var worldPlayerPair = _worldGroup.SectionPlayerDict[section];
                var worldEnemyPair = _worldGroup.SectionEnemyDict[section];

                // 유닛이 없는 경우 미처리.
                if (false == worldPlayerPair.Any() && false == worldEnemyPair.Any())
                {
                    return;
                }

                /////////////////////////////////////////////////////////////
                // 플레이어 이동 처리.
                foreach (var wp in worldPlayerPair.Values)
                {
                    var playInfo = wp.PlayInfo;
                    var location = playInfo.Location;

                    // 정지, 대시인 경우, 처리하지 않음.
                    switch (playInfo.Action)
                    {
                        case Type_Action.Move:          // 이동 처리.
                            var deltaPos = location.MoveDirection * (float)deltaTime * playInfo.Stat.Velocity;
                            var movePos = location.Pos + deltaPos;

                            if (false == IsValidSection(movePos))
                            {
                                _logger.LogError($"invalid position. pos: {movePos}");
                                continue;
                            }

                            // 위치 갱신
                            location.Pos = movePos;
                            wp.Position = movePos;
                            break;
                        case Type_Action.MoveEnd:       // 이동 위치 평가
                        case Type_Action.Knockback:
                        case Type_Action.Dash:
                            // 상태 갱신
                            playInfo.Action = Type_Action.None;
                            // 위치 갱신
                            location.Pos = location.PosEnd;
                            wp.Position = location.PosEnd;
                            break;
                        default:
                            continue;
                    }

                    // 변경된 섹션 검사.
                    var moveSection = SectionId.GetSection(location.Pos);

                    //_logger.LogDebug($"player position. pos: {location.Position}");

                    if (false == section.Equals(moveSection))
                    {
                        wp.OldSection = wp.Section;
                        wp.Section = moveSection;
                        
                        // Remove from section.
                        _worldGroup.SectionPlayerDict[section].TryRemove(wp.Pid, out var _);

                        // 관리 섹션 검사.
                        if (IsManagedSection(moveSection))
                        {
                            // 섹션 이동.
                            _worldGroup.SectionPlayerDict[moveSection].AddOrUpdate(wp.Pid, wp, (_, _) => wp);

                            moveSectionUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_SectionUnit>())
                            .PlayInfoDict.AddOrUpdate(wp.Pid, playInfo, (_, _) => playInfo);
                        }
                        else
                        {
                            // 구역 이동.
                            moveDistrictUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_DistrictUnit>())
                            .PlayerEntities.Add(wp);

                            // 구역에서 플레이어 제거.
                            _worldGroup.PlayerDict.Remove(wp.Pid);
                        }
                    }
                }

                /////////////////////////////////////////////////////////////
                // 적 이동 처리.
                foreach (var we in worldEnemyPair.Values)
                {
                    var enemy = we.Enemy;
                    var location = enemy.Location;

                    // 적 위치 계산.
                    var movePos = Calc_EnemyPosition(location.Pos, location.NextPos, location.NextPosDir, (float)deltaTime, location.Veltocity);
                    var moveSection = SectionId.GetSection(movePos);

                    // 위치 갱신.
                    location.Pos = movePos;
                    we.Position = movePos;

                    // 섹션 업데이트.
                    if (false == section.Equals(moveSection))
                    {
                        we.Section = moveSection;

                        // remove from old section.
                        _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var _);

                        if (IsManagedSection(moveSection))
                        {
                            // 섹션 이동.
                            _worldGroup.SectionEnemyDict[moveSection].AddOrUpdate(enemy.EnemeyUid, we, (_, _) => we);

                            moveSectionUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_SectionUnit>())
                            .Enemies.Add(enemy);
                        }
                        else
                        {
                            // 구역 이동.
                            moveDistrictUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_DistrictUnit>())
                            .EnemyEntities.Add(we);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Calculation position of enemy.
        /// </summary>
        private Vector2 Calc_EnemyPosition(Vector2 oriPos, Vector2 nextPos, Vector2 moveDir, float frameDeltaTime, int velocity)
        {
            // X 이동.
            if (moveDir.X > 0)
            {
                // 목표 위치가 오른쪽인 경우.
                // 현재 위치가 더 왼쪽인 경우만 처리.
                if (oriPos.X < nextPos.X) { oriPos.X += moveDir.X * frameDeltaTime * velocity; }
            }
            else if (moveDir.X < 0)
            {
                // 목표 위치가 왼쪽인 경우.
                // 현재 위치가 더 오른쪽인 경우만 처리.
                if (oriPos.X > nextPos.X) { oriPos.X += moveDir.X * frameDeltaTime * velocity; }
            }

            // Y 이동.
            if (moveDir.Y > 0)
            {
                // 목표 위치가 위인 경우.
                // 현재 위치가 더 아래인 경우만 처리.
                if (oriPos.Y < nextPos.Y) { oriPos.Y += moveDir.Y * frameDeltaTime * velocity; }
            }
            else if (moveDir.Y < 0)
            {
                // 목표 위치가 아래인 경우.
                // 현재 위치가 더 위인 경우만 처리.
                if (oriPos.Y > nextPos.Y) { oriPos.Y += moveDir.Y * frameDeltaTime * velocity; }
            }

            return oriPos;
        }
    }
}
