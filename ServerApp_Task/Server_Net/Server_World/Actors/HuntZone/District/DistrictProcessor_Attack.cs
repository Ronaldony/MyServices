using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_World.Actors.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Collision;
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 플레이어 공격 처리.
        /// </summary>
        private void Attack(World_Player worldPlayer, NetPacketCS_Attack packet)
        {
            var nowTime = TimeUtil.Now;

            // 플레이어.
            var playerInfo = worldPlayer.PlayInfo;
            var pos = playerInfo.Location.Position;

            // 무기 정보.
            var tbWeapon = _tdbService.GetById<TB_Weapon>(playerInfo.Equipment.Weapon);

            // 데미지 검증.
            if (tbWeapon.Damage != packet.Damage)
            {
                _logger.LogError($"not exist player. pid: {playerInfo.Pid}");
                return;
            }

            // 충돌 박스.
            var collision = _collisionService.Rent(tbWeapon.CollisionType);
            var bounding = collision.GetBoudingBox(pos.X, pos.Y, packet.Angle);

            // 공격 이벤트 전달.
            RouteGameEvent(new AttackEvent
            {
                Damage = tbWeapon.Damage,
                AttackTime = nowTime,
                BoudingBox = new BoudingBox
                {
                    MaxX = bounding.MaxX,
                    MaxY = bounding.MaxY,
                    MinX = bounding.MinX,
                    MinY = bounding.MinY,
                }
            });

            // 공격 패킷 전송.
            SendPacket_AroundSections(worldPlayer.GameSession, worldPlayer.Section, NetPacketCode.SC_Attack, new NetPacketSC_Attack
            {
                Pid = playerInfo.Pid,
                Angle = packet.Angle
            });

            // 근처 섹션 enemy 충돌 검사.
            var hitEnemies = new List<DTO_Enemy>();

            foreach (var hitSection in _worldGroup.ManagedSections)
            {
                HitEnemies(tbWeapon.Damage, hitSection, hitEnemies, bounding, nowTime);
            }

            // 피격 패킷 전송.
            SendPacket_AroundSections(worldPlayer.GameSession, worldPlayer.Section, NetPacketCode.SC_HitEnemy, new NetPacketSC_HitEnemy
            {
                Damage = tbWeapon.Damage,
                Position = pos,
                Enemies = hitEnemies
            });

            // Release.
            _collisionService.Release(collision);
        }

        /// <summary>
        /// Hit enemies.
        /// </summary>
        private void HitEnemies(int damage, SectionId section, List<DTO_Enemy> hitEnemies, BoudingBox weaponBouding, DateTime hitTime)
        {
            var enemies = _worldGroup.EnemyDict.Values;

            if (false == enemies.Any())
            {
                return;
            }

            var deathEnemies = new List<DTO_Enemy>();

            // Enemy 충돌 계산.
            foreach (var worldEnemy in enemies)
            {
                var enemy = worldEnemy.Enemy;

                // 피격 딜레이 계산.
                if (false == TimeUtil.IsPastTime(enemy.HitTime.AddMilliseconds(GameData.HIT_DELAY_MS), hitTime))
                {
                    continue;
                }

                enemy.HitTime = hitTime;

                var pos = enemy.Location.Pos;
                var targetBouding = worldEnemy.CollisionBox.GetBoudingBox(pos.X, pos.Y);

                // 충돌 판정.
                if (true == _collisionService.CollidesWith(weaponBouding, targetBouding))
                {
                    _logger.LogWarning($"Hit enemy. pos: {pos}, min: ({targetBouding.MinX}, {targetBouding.MinY}), max: ({targetBouding.MaxX}, {targetBouding.MaxY})");

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

            // 적 유닛 죽음 처리.
            foreach (var enemy in deathEnemies)
            {
                _worldGroup.EnemyDict.Remove(enemy.EnemeyUid);
                _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var worldEnemy);
                _collisionService.Release(worldEnemy.CollisionBox);
            }
        }
    }
}
