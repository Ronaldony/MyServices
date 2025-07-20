using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using Server_World.Core.GameEvents;
    using Server_World.Core.Projectiles.Players;
    using Server_World.Core.Worlds;
    using ServerEngine.Core.Util;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - 공격 처리.
        /// </summary>
        private void UpdateFrame_Attack(float deltaTime, ConcurrentBag<World_UpdateFrame> updateFrames)
        {
            // 적 시야 감지 및 공격 처리.
            // 적 공격 - 플레이어 충돌 처리.
        }

        /// <summary>
        /// 프레임 업데이트 - 플레이어 투사체 처리.
        /// </summary>
        private void UpdateFrame_Projectiles(float deltaTime)
        {
            var nowTime = TimeUtil.Now;

            // 생성.
            foreach (var projectile in _worldProjectile.SpawnProjectiles)
            {
                _worldProjectile.Projectiles.Add(projectile);
            }

            // 제거할 투사체, 피격 적 유닛.
            var removeProjectiles = new List<Projectile>();
            var moveProjectileEvent = new MoveProjectileDistrict();
            var deathenemies = new List<World_Enemy>();

            // 투사체 처리.
            foreach (var projectile in _worldProjectile.Projectiles)
            {
                var hitbox = projectile.GetHitbox();
                var worldEnemies = _worldGroup.SectionEnemyDict[projectile.Section].Values;

                // 적 유닛 충돌 처리.
                foreach (var we in worldEnemies)
                {
                    var enemyHitbox = we.CollisionBox.GetHitbox();

                    if (hitbox.IsHit(enemyHitbox))
                    {
                        // 충돌.
                        deathenemies.Add(we);
                    }
                }

                // 투사체 이동.
                projectile.MoveNextFrame(deltaTime);

                // 최대 이동 거리 검사.
                if (projectile.IsMaxRange())
                {
                    removeProjectiles.Add(projectile);
                }
                else
                {
                    // 구역 이동 검사.
                    if (IsManagedSection(projectile.Section))
                    {
                        moveProjectileEvent.Projectiles.Add(projectile);
                    }
                }
            }

            // 충돌한 적 유닛 처리.
            foreach (var we in deathenemies)
            {
                // 피격 딜레이 검사.
                if (TimeUtil.IsPastTime(we.Enemy.HitTime.AddMilliseconds(GameData.HIT_DELAY_MS), nowTime))
                {

                }
            }

            // 투사체 제거.
            foreach (var projectile in removeProjectiles)
            {
                _worldProjectile.Projectiles.Remove(projectile);
            }
        }

        /// <summary>
        /// 프레임 업데이트 - 적 투사체 처리.
        /// </summary>
        private void UpdateFrame_EnemyProjectiles()
        {

        }
    }
}
