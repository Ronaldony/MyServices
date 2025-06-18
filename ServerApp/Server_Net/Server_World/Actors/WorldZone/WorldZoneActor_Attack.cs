
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Collision;
    using Server_World.Core.Services;
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
            var position = playerInfo.Location.Position;

            // 무기 정보.
            var tbWeapon = _tdbService.GetById<TB_Weapon>(playerInfo.Equipment.Weapon);

            // 데미지 검증.
            if (tbWeapon.Damage != packet.Damage)
            {
                _logger.LogError($"not exist player. pid: {playerInfo.Pid}");
                return;
            }

            // 섹션 정보.
            var section = GetSection(position);

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
            using var enemyCollision = CollisionManager.Rent(Type_Collision.Enemy);
            using var weaponCollision = CollisionManager.Rent(tbWeapon.CollisionType);
            var weaponBounding = weaponCollision.GetBoudingBox(position.X, position.Y, packet.Angle);

            // 근처 섹션 enemy 충돌 검사.
            var hitEnemies = new List<DTO_Enemy>();
            var aroundSections = GetAroundSections(section);

            foreach (var hitSection in aroundSections)
            {
                HitEnemies(tbWeapon.Damage, hitSection, hitEnemies, weaponBounding, enemyCollision, nowTime);
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
                    Position = position,
                    Enemies = hitEnemies
                }));
        }

        /// <summary>
        /// Hit enemies.
        /// </summary>
        private void HitEnemies(int damage, (int, int) section, List<DTO_Enemy> hitEnemies, BoudingBox weaponBouding, CollisionBox enemyCollision, DateTime hitTime)
        {
            var enemies = GetSectionEnemies(section);

            if (null == enemies)
            {
                return;
            }

            // Death enemeis.
            var deathEnemies = new List<DTO_Enemy>();

            // Enemy 충돌 계산.
            foreach (var enemy in enemies)
            {
                // 피격 딜레이 계산.
                if (false == TimeUtil.IsPastTime(enemy.HitTime.AddMilliseconds(GameData.HIT_DELAY_MS), hitTime))
                {
                    continue;
                }

                enemy.HitTime = hitTime;

                var pos = enemy.Position.Pos;
                var targetBouding = enemyCollision.GetBoudingBox(pos.X, pos.Y);

                // 충돌 판정.
                if (true == CollisionManager.CollidesWith(weaponBouding, targetBouding))
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
                _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out _);
            }
        }
    }
}
