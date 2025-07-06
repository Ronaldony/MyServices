
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Collision;
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
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

            // 섹션 정보.
            var section = GetSection(pos);

            // 공격 패킷 전송.
            SendPacket_AroundSections(
                ignoreSession: worldPlayer.GameSession,
                section: section,
                packetCode: NetPacketCode.SC_Attack,
                packetBytes: _dataSerializer.Serialize(new NetPacketSC_Attack
                {
                    Pid = playerInfo.Pid,
                    Angle = packet.Angle
                }));

            // 충돌 박스.
            var weaponCollision = _collisionService.Rent(tbWeapon.CollisionType);
            var weaponBounding = weaponCollision.GetBoudingBox(pos.X, pos.Y, packet.Angle);

            // 근처 섹션 enemy 충돌 검사.
            var hitEnemies = new List<DTO_Enemy>();
            var aroundSections = GetAroundSections(section);

            foreach (var hitSection in aroundSections)
            {
                HitEnemies(tbWeapon.Damage, hitSection, hitEnemies, weaponBounding, nowTime);
            }

            // 피격 패킷 전송.
            SendPacket_AroundSections(
                ignoreSession: worldPlayer.GameSession,
                section: section,
                packetCode: NetPacketCode.SC_HitEnemy,
                packetBytes: _dataSerializer.Serialize(
                new NetPacketSC_HitEnemy
                {
                    Damage = tbWeapon.Damage,
                    Position = pos,
                    Enemies = hitEnemies
                }));

            // Release.
            _collisionService.Release(weaponCollision);
        }

        /// <summary>
        /// Hit enemies.
        /// </summary>
        private void HitEnemies(int damage, SectionId section, List<DTO_Enemy> hitEnemies, BoudingBox weaponBouding, DateTime hitTime)
        {
            var enemies = GetSectionEnemies(section);

            if (false == enemies.Any())
            {
                return;
            }

            // Death enemeis.
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

                    if (enemy.Health <= 0)
                    {
                        // 죽음.
                        deathEnemies.Add(enemy);
                    }
                }
            }

            // Enemy 죽음 처리.
            foreach (var enemy in deathEnemies)
            {
                _logger.LogWarning($"Death enemy.");

                _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var worldEnemy);
                _collisionService.Release(worldEnemy.CollisionBox);
            }
        }
    }
}
