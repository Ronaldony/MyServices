using ServerEngine.Core.Util;
using System.Collections.Concurrent;
using System.Numerics;

namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Update;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 공격 처리.
        /// </summary>
        private void UpdateFrame_Collision(double deltaTime, ConcurrentDictionary<SectionId, WorldUpdate_HitUnit> hitUnits)
        {
            // 투사체 업데이트.
            UpdateFrame_CollisionPlayer(deltaTime, hitUnits);

            // Enemy 플레이어 감지.
            //UpdateFrame_EnemyScanPlayer(deltaTime, updateFrame);
        }

        /// <summary>
        /// 프레임 업데이트 - 플레이어 투사체 처리.
        /// </summary>
        private void UpdateFrame_CollisionPlayer(double deltaTime, ConcurrentDictionary<SectionId, WorldUpdate_HitUnit> hitUnits)
        {
            foreach (var pu in _worldGroup.PlayerDict.Values)
            {
                // Check projectiles.
                if (pu.CombatHandler.HasProjectiles())
                {
                    var arounds = GetAroundSections(pu.MovementHandler.Section());
                }
            }

            var hitSectionUnit = updateFrame.HitUnits;
            var moveDistrictProjectile = updateFrame.MoveDistrictProjectiles;

            // 투사체 생성.
            foreach (var projectile in _worldProjectile.SpawnProjectiles)
            {
                _worldProjectile.Projectiles.Add(projectile);
            }

            var updateProjectile = _memoryPoolService.Rent<WorldUpdate_Projectile>();

            // 투사체 처리.
            foreach (var projectile in _worldProjectile.Projectiles)
            {
                var hitbox = projectile.GetHitbox();
                var enemyUnits = _worldGroup.WorldSectionDict[projectile.Section].GetMobUnits();

                // 적과 투사체 충돌 처리.
                foreach (var eu in enemyUnits)
                {
                    var enemyHitbox = eu.UnitCollision.GetHitbox();

                    // 충돌 검사.
                    if (hitbox.IsHit(enemyHitbox))
                    {
                        var hitSectionUnits = hitSectionUnit.GetOrAdd(eu.Section, section => new ());
                        hitSectionUnits.Enemies.Add(eu.Mob);

                        // 데미지.
                        eu.Mob.Health -= projectile.Damage;

                        if (eu.Mob.Health < 0)
                        {
                            updateProjectile.DeathEnemies.Add(eu);
                        }
                    }
                }

                // 투사체 이동.
                projectile.MoveNextFrame(deltaTime);

                if (projectile.IsMaxRange())
                {
                    // 최대 이동 거리 초과.
                    updateProjectile.RemoveProjectiles.Add(projectile);
                }
                else
                {
                    // 투사체가 구역을 이동한 경우, 해당 구역에 전달.
                    if (IsManagedSection(projectile.Section))
                    {
                        var moveProjectiles = moveDistrictProjectile.GetOrAdd(projectile.Section, section => new WorldUpdate_DistrictProjectile());
                        moveProjectiles.Projectiles.Add(projectile);
                    }
                }
            }

            // 투사체 제거.
            foreach (var projectile in updateProjectile.RemoveProjectiles)
            {
                _worldProjectile.Projectiles.Remove(projectile);
            }

            // 죽은 적 처리.
            foreach (var ee in updateProjectile.DeathEnemies)
            {
                _worldGroup.WorldSectionDict[ee.Section].LeaveMob(ee.UnitId);
                _collisionService.Release(ee.UnitCollision);
            }

            // Relase.
            _memoryPoolService.Release(updateProjectile);
        }

        /// <summary>
        /// 프레임 업데이트 - 적 투사체 처리.
        /// </summary>
        private void UpdateFrame_EnemyScanPlayer(double deltaTime, WorldUpdate_Frame updateFrame)
        {
            var nowTime = TimeUtil.Now;

            // Enemy 시야 스캔.
            foreach (var wePair in _worldGroup.SectionMobDict.Values)
            {
                foreach (var we in wePair.Values)
                {
                    // 액션 검사.
                    switch (we.Mob.Action)
                    {
                        case Type_EnemyAction.Track:
                            // 현재 위치가 스폰 지점에서 너무 먼 경우 Return 상태로 변경.
                            break;
                        case Type_EnemyAction.Return:
                            // 현재 위치와 스폰 위치가 동일한 경우 상태 업데이트.
                            break;
                        default:
                            // 플레이어 스캔.
                            foreach (var wp in _worldGroup.PlayerDict.Values)
                            {
                                if (GameData.ENEMY_SCAN_DISTANCE_SQUARE > (int)Vector2.DistanceSquared(wp.Position, we.Position))
                                {
                                    // 추적대상 감지.
                                    we.Mob.Action = Type_EnemyAction.Track;
                                    we.Mob.TrackPid = wp.Pid;
                                    we.Mob.Location.NextPos = wp.Position;
                                    break;
                                }
                            }

                            // 추적대상 감지됨.
                            if (we.Mob.Action == Type_EnemyAction.Track)
                            {
                                continue;
                            }

                            // 외부 섹션 플레이어 스캔
                            var aroundSections = _router.GetAroundSections(we.Section);

                            foreach (var pair in _outsideWorldPlayers)
                            {
                                if (false == aroundSections.Contains(pair.Key))
                                {
                                    continue;
                                }

                                var scanOutsidePlayers = pair.Value.Where(wp => GameData.ENEMY_SCAN_DISTANCE_SQUARE > (int)Vector2.DistanceSquared(wp.Position, we.Position));

                                foreach (var wp in pair.Value)
                                {
                                    if (GameData.ENEMY_SCAN_DISTANCE_SQUARE > (int)Vector2.DistanceSquared(wp.Position, we.Position))
                                    {
                                        // 플레이어 감지.
                                        we.Mob.Action = Type_EnemyAction.Track;
                                        we.Mob.TrackPid = wp.Pid;
                                        we.Mob.Location.NextPos = wp.Position;    // 원거리 공격인 경우엔?
                                        break;
                                    }
                                }

                                // 추적대상 감지됨.
                                if (we.Mob.Action == Type_EnemyAction.Track)
                                {
                                    break;
                                }
                            }
                            break;
                    }

                }
            }
            // 적 공격 - 플레이어 충돌 처리.
        }
    }
}
