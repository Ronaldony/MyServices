using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.GamePacket;
    using Server_Realm.Entities;
    using Server_Realm.GameEvents;
    using Server_Realm.HitBoxes;
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 플레이어 공격 처리.
        /// </summary>
        private void Attack(PlayerUnit pe, GamePacketCS_Attack packet)
        {
            var nowTime = TimeUtil.Now;

            // 플레이어.
            var playerInfo = pe.PlayInfo;
            var pos = pe.Position;

            // 무기 정보.
            var tbWeapon = _tdbService.GetById<TB_Weapon>(playerInfo.Equipment.Weapon);

            // 충돌 박스.
            var collision = _collisionService.Rent(tbWeapon.CollisionType);
            collision.Update(pos, packet.Angle);

            var hitbox = collision.GetHitbox();

            // 공격 패킷.
            var attackPacket = new GamePacketSC_Attack
            {
                Pid = pe.Pid,
                Angle = packet.Angle
            };

            // 공격 이벤트 전달.
            RouteGameEvent(new AttackEvent
            {
                GamePacket = attackPacket,
                Damage = tbWeapon.Damage,
                AttackTime = nowTime,
                RouteSection = pe.Section,
                Angle = packet.Angle,
                AttackerPos = pos,
                WeaponCollisionType = tbWeapon.CollisionType,
            });

            // 공격 패킷 전송.
            SendPacket_AroundSections(pe.Pid, pe.Section, GamePacketCode.SC_Attack, attackPacket);

            // 근처 섹션 enemy 충돌 검사.
            var hitEnemies = new List<DTO_Enemy>();

            foreach (var hitSection in _worldGroup.ManagedSections)
            {
                // TODO: Attack entity.
                HitEnemies(tbWeapon.Damage, hitSection, hitEnemies, hitbox, nowTime);
            }

            // 피격 패킷 전송.
            if(hitEnemies.Any())
            {
                SendPacket_AroundSections(pe.Pid, pe.Section, GamePacketCode.SC_HitEnemy, new GamePacketSC_HitEnemy
                {
                    Damage = tbWeapon.Damage,
                    Enemies = hitEnemies
                });
            }
            
            // Release.
            _collisionService.Release(collision);
        }

        /// <summary>
        /// Hit enemies.
        /// </summary>
        private void HitEnemies(int damage, SectionId section, List<DTO_Enemy> hitEnemies, IHitbox weaponHitbox, DateTime hitTime)
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
                    var collision = entities.Collision;
                    entities.Collision.Update(enemy.Location.Pos);

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
                _collisionService.Release(entity.Collision);
            }
        }
    }
}
