using ServerEngine.Core.Util;

namespace Server_World.Actors.HuntZone.District
{
    using Server_World.Core.Projectiles.Players;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Section;
    using Server_World.Core.Worlds.Update;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 공격 처리.
        /// </summary>
        private void UpdateFrame_Attack(float deltaTime, World_UpdateFrame updateFrame)
        {
            // 투사체 업데이트.
            UpdateFrame_Projectiles(deltaTime, updateFrame);

            // 적 투사체 업데이트.
            UpdateFrame_EnemyProjectiles(deltaTime, updateFrame);
        }

        /// <summary>
        /// 프레임 업데이트 - 플레이어 투사체 처리.
        /// </summary>
        private void UpdateFrame_Projectiles(float deltaTime, World_UpdateFrame updateFrame)
        {
            var nowTime = TimeUtil.Now;

            // 투사체 생성.
            foreach (var projectile in _worldProjectile.SpawnProjectiles)
            {
                _worldProjectile.Projectiles.Add(projectile);
            }

            var updateProjectile = _memoryPoolService.Rent<Update_Projectile>();

            // 투사체 처리.
            foreach (var projectile in _worldProjectile.Projectiles)
            {
                var hitbox = projectile.GetHitbox();
                var worldEnemies = _worldGroup.SectionEnemyDict[projectile.Section].Values;

                // 적과 투사체 충돌 처리.
                foreach (var we in worldEnemies)
                {
                    var enemyHitbox = we.CollisionBox.GetHitbox();

                    // 충돌 검사.
                    if (hitbox.IsHit(enemyHitbox))
                    {
                        var hitSectionUnits = updateFrame.HitSectionUnit.GetOrAdd(we.Section, section => new World_SectionUnit());
                        hitSectionUnits.Enemies.Add(we.Enemy);

                        // 데미지.
                        we.Enemy.Health -= projectile.Damage;

                        if (we.Enemy.Health < 0)
                        {
                            updateProjectile.DeathEnemies.Add(we);
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
                        var moveProjectiles = updateFrame.MoveDistrictProjectile.GetOrAdd(projectile.Section, section => new List<Projectile>());
                        moveProjectiles.Add(projectile);
                    }
                }
            }

            // 투사체 제거.
            foreach (var projectile in updateProjectile.RemoveProjectiles)
            {
                _worldProjectile.Projectiles.Remove(projectile);
            }

            // 죽은 적 처리.
            foreach (var we in updateProjectile.DeathEnemies)
            {
                _worldGroup.SectionEnemyDict[we.Section].Remove(we.EnemyUid, out _);
                _collisionService.Release(we.CollisionBox);
            }

            // Relase.
            _memoryPoolService.Release(updateProjectile);
        }

        /// <summary>
        /// 프레임 업데이트 - 적 투사체 처리.
        /// </summary>
        private void UpdateFrame_EnemyProjectiles(float deltaTime, World_UpdateFrame updateFrame)
        {
            // 적 시야 감지 및 공격 처리.
            foreach (var pair in _worldGroup.SectionEnemyDict)
            {
                var weDict = pair.Value;

                foreach (var we in weDict.Values)
                {

                }
            }

            // 적 공격 - 플레이어 충돌 처리.
        }
    }
}
