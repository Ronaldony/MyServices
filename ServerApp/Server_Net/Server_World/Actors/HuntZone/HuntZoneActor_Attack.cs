
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Collision;
    using ServerEngine.Core.Util;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 플레이어 공격 처리.
        /// </summary>
        private void Attack(NetPacketCS_Attack packet)
        {
            var nowTime = TimeUtil.Now;
            var worldPlayer = GetPlayerOrDefault(packet.Pid);

            if (default == worldPlayer)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}");
                return;
            }

            // 플레이어.
            var playerInfo = worldPlayer.PlayInfo;
            var playerPos = playerInfo.Position.Pos;

            // 무기 정보.
            var damage = _weaponService.GetWeaponDamage(playerInfo.Equipment.Weapon);

            // 데미지 검증.
            if (damage != packet.Damage)
            {
                _logger.LogError($"not exist player. pid: {packet.Pid}");
                return;
            }

            // 섹션 정보.
            var section = ((int)playerPos.X / GameData.SECTION_WIDTH, (int)playerPos.Y / GameData.SECTION_HEIGHT);

            // 공격 패킷 전송.
            SendPacket_AroundSections(
                ignoreSession: worldPlayer.GameSession,
                section: section,
                packetCode: NetPacketCode.SC_Attack,
                packetBytes: _dataSerializer.Serialize(new NetPacketSC_Attack
                {
                    Pid = packet.Pid,
                    Angle = packet.Angle
                }));

            // 충돌 박스.
            var enemyCollision = _collisionService.Rent(Type_Collision.Enemy);
            var weaponCollision = _collisionService.Rent_Weapon(playerInfo.Equipment.Weapon);
            var weaponBounding = weaponCollision.GetBoudingBox(playerPos.X, playerPos.Y, packet.Angle);

            // 근처 섹션 enemy 충돌 검사.
            var hitEnemies = new List<DTO_Enemy>();
            var aroundSections = GetAroundSections(section);

            foreach (var hitSection in aroundSections)
            {
                HitEnemies(damage, hitSection, hitEnemies, weaponBounding, enemyCollision, nowTime);
            }

            // Release.
            _collisionService.Release(enemyCollision);
            _collisionService.Release(weaponCollision);

            // 피격 패킷 전송.
            SendPacket_AroundSections(
                ignoreSession: worldPlayer.GameSession,
                section: section,
                packetCode: NetPacketCode.SC_HitEnemy,
                packetBytes: _dataSerializer.Serialize(
                new NetPacketSC_HitEnemy
                {
                    Damage = damage,
                    Position = playerPos,
                    Enemies = hitEnemies
                }));
        }

        /// <summary>
        /// Attack enemies.
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
                _sectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out _);
            }
        }
    }
}
