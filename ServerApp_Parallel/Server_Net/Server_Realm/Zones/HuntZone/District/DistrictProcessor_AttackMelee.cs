using Microsoft.Extensions.Logging;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.GamePacket;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 플레이어 근접공격 처리.
        /// </summary>
        private void AttackMelee(PlayerUnit pu, GamePacketCS_AttackMelee packet)
        {
            var total = _worldGroup.SectionEnemyDict.Values.Count();
            var enemyUnits = new List<Unit>(total);

            foreach (var pair in _worldGroup.SectionEnemyDict.Values)
            {
                enemyUnits.AddRange(pair.Values);
            }

            // 근정 공격.
            var hitEnemies = pu.AttackMelee(packet.Angle, _worldGroup.EnemyDict.Values);

            //공격 패킷 전송.
            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_Attack,
                new GamePacketSC_AttackMelee
                {
                    Pid = pu.Pid,
                    Angle = packet.Angle
                });

            // 피격 패킷 전송.
            if (hitEnemies.Any())
            {
                SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_HitEnemy, new GamePacketSC_HitEnemy
                {
                    //Damage = .Damage,
                    //Enemies = hitEnemies
                });
            }
        }

        /// <summary>
        /// Hit enemies.
        /// </summary>
        private void HitEnemies(int damage, SectionId section, List<DTO_Enemy> hitEnemies, IHitbox weaponHitbox)
        {
            var deathEnemies = new List<DTO_Enemy>();

            foreach (var pair in _worldGroup.SectionEnemyDict.Values)
            {
                var enemies = pair.Values;

                // Enemy 충돌 계산.
                foreach (var entities in enemies)
                {
                    var enemy = entities.Enemy;

                    // Update collision.
                    var collision = entities.UnitCollision;
                    entities.UnitCollision.Update(enemy.Location.Pos);

                    var enemyHitbox = collision.GetHitbox();

                    // 충돌 판정.
                    if (true == weaponHitbox.IsHit(enemyHitbox))
                    {
                        _logger.LogWarning($"Hit enemy. pos: {enemy.Location.Pos}");

                        hitEnemies.Add(enemy);
                        enemy.Health -= damage;

                        // 죽음 처리.
                        if (enemy.Health <= 0)
                        {
                            _logger.LogWarning($"Death enemy.");

                            // Death enemy.
                            deathEnemies.Add(enemy);
                        }
                    }
                }
            }

            // 적 유닛 죽음 처리.
            foreach (var enemy in deathEnemies)
            {
                _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var entity);
                _collisionService.Release(entity.UnitCollision);
            }
        }
    }
}
