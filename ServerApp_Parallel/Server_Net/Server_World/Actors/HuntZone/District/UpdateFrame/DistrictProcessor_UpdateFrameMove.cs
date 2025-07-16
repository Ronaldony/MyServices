using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using Server_World.Core.Datas;
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
        private void UpdateFrame_Move(float frameIntervalSec, ConcurrentBag<World_UpdateFrame> updateFrames)
        {
            // 플레이어가 존재하는 섹션의 플레이어, 적 이동 처리.
            Parallel.ForEach(_worldGroup.ManagedSections, section =>
            {
                // 섹션 유닛.
                var worldPlayers = _worldGroup.SectionPlayerDict[section];
                var worldEnemies = _worldGroup.SectionEnemyDict[section];

                // 유닛이 없는 경우 미처리.
                if (false == worldPlayers.Any() && false == worldEnemies.Any())
                {
                    return;
                }

                // 업데이트 프레임 섹션.
                var worldUpdateFrame = _memoryPoolService.Rent<World_UpdateFrame>();
                var moveSectionUnit = worldUpdateFrame.MoveSectionUnit;
                var moveDistrictUnit = worldUpdateFrame.MoveDistrictUnit;

                updateFrames.Add(worldUpdateFrame);

                /////////////////////////////////////////////////////////////
                // 플레이어 이동 처리.
                foreach (var worldPlayer in worldPlayers.Values)
                {
                    var playInfo = worldPlayer.PlayInfo;
                    var location = playInfo.Location;

                    // 정지, 대시인 경우, 처리하지 않음.
                    switch (playInfo.Action)
                    {
                        case Type_Action.Move:          // 이동 처리.
                            var movePos = Calc_PlayerPosition(location.Position, location.MoveDirection, frameIntervalSec, playInfo.Stat.Velocity);

                            if (false == IsValidSection(movePos))
                            {
                                _logger.LogError($"invalid position. pos: {movePos}");
                                continue;
                            }

                            location.Position = movePos;
                            break;
                        case Type_Action.MoveEnd:       // 이동 위치 평가
                        case Type_Action.Knockback:
                        case Type_Action.Dash:
                            // 위치 개인.
                            playInfo.Action = Type_Action.None;
                            location.Position = location.PositionEnd;
                            break;
                        default:
                            continue;
                    }

                    // 변경된 섹션 검사.
                    var moveSection = SectionId.GetSection(location.Position);

                    //_logger.LogDebug($"player position. pos: {location.Position}");

                    if (false == section.Equals(moveSection))
                    {
                        worldPlayer.OldSection = worldPlayer.Section;
                        worldPlayer.Section = moveSection;
                        
                        // Remove from section.
                        _worldGroup.SectionPlayerDict[section].TryRemove(worldPlayer.Pid, out var _);

                        // 관리 섹션 검사.
                        if (IsManagedSection(moveSection))
                        {
                            // 섹션 이동.
                            _worldGroup.SectionPlayerDict[moveSection].AddOrUpdate(worldPlayer.Pid, worldPlayer, (_, _) => worldPlayer);

                            moveSectionUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_SectionUnit>())
                            .PlayInfoDict.AddOrUpdate(worldPlayer.GameSession, playInfo, (_, _) => playInfo);
                        }
                        else
                        {
                            // 구역 이동.
                            moveDistrictUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_DistrictUnit>())
                            .WorldPlayers.Add(worldPlayer);
                        }
                    }
                }

                /////////////////////////////////////////////////////////////
                // 적 이동 처리.
                foreach (var wolrdEnemy in worldEnemies.Values)
                {
                    var enemy = wolrdEnemy.Enemy;
                    var location = enemy.Location;

                    // 적 위치 계산.
                    var movePos = Calc_EnemyPosition(location.Pos, location.NextPos, location.NextPosDir, frameIntervalSec, location.Veltocity);
                    var moveSection = SectionId.GetSection(movePos);

                    // 위치 업데이트.
                    location.Pos = movePos;

                    // 섹션 업데이트.
                    if (false == section.Equals(moveSection))
                    {
                        wolrdEnemy.Section = moveSection;

                        // remove from old section.
                        _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var _);

                        if (IsManagedSection(moveSection))
                        {
                            // 섹션 이동.
                            _worldGroup.SectionEnemyDict[moveSection].AddOrUpdate(enemy.EnemeyUid, wolrdEnemy, (_, _) => wolrdEnemy);

                            moveSectionUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_SectionUnit>())
                            .Enemies.Add(enemy);
                        }
                        else
                        {
                            // 구역 이동.
                            moveDistrictUnit
                            .GetOrAdd(moveSection, _memoryPoolService.Rent<World_DistrictUnit>())
                            .WorldEnemies.Add(wolrdEnemy);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Calculation position of player.
        /// </summary>
        private Vector2 Calc_PlayerPosition(Vector2 oriPos, Type_MoveDirection moveDir, double diffSeconds, int velocity)
        {
            // 이동 방향 계산 시퀀스. 
            // 1. 이동 방향 확인.
            // 2. 이동 방향에 따라 이동 오차 범위 값 계산.
            // 3. x, y 좌표에 이동 오차 범위 값을 더하여 범위 내인 경우 true, 범위 밖인 경우 false.
            switch (moveDir)
            {
                case Type_MoveDirection.Top:
                    {
                        oriPos.Y += (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        oriPos.Y -= (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        oriPos.X += (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        oriPos.X -= (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X += calcDist;
                        oriPos.Y += calcDist;
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X -= calcDist;
                        oriPos.Y += calcDist;
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X += calcDist;
                        oriPos.Y -= calcDist;
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X -= calcDist;
                        oriPos.Y -= calcDist;
                    }
                    break;
            }

            return oriPos;
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
