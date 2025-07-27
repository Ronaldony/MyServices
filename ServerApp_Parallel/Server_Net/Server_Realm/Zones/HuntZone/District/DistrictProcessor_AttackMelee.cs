namespace Server_Realm.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;
    using Server_Realm.GameEvents;

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
            var total = _worldGroup.SectionMobDict.Values.Count();
            var enemyUnits = new List<Unit>(total);

            foreach (var pair in _worldGroup.SectionMobDict.Values)
            {
                enemyUnits.AddRange(pair.Values);
            }

            // 근정 공격.
            pu.AttackMelee(packet.Angle, _worldGroup.MobDict.Values);

            // 공격 이벤트 전달.
            RouteGameEvent(new AttackMeleeEvent
            {
                PlayerUnit = pu,
                Angle = packet.Angle
            });

            //공격 패킷 전송.
            SendPacket_AroundSections(pu.Pid, pu.Section, GamePacketCode.SC_Attack,
                new GamePacketSC_AttackMelee
                {
                    Pid = pu.Pid,
                    Angle = packet.Angle
                });
        }

        /// <summary>
        /// Hit enemies.
        /// </summary>
        //private void HitEnemies(int damage, SectionId section, List<DTO_Enemy> hitEnemies, IHitbox weaponHitbox)
        //{
        //    var deathEnemies = new List<DTO_Enemy>();

        //    foreach (var pair in _worldGroup.SectionEnemyDict.Values)
        //    {
        //        var enemies = pair.Values;

        //        // Enemy 충돌 계산.
        //        foreach (var entities in enemies)
        //        {
        //            var enemy = entities.Enemy;

        //            // Update collision.
        //            var collision = entities.UnitCollision;
        //            entities.UnitCollision.Update(enemy.Location.Pos);

        //            var enemyHitbox = collision.GetHitbox();

        //            // 충돌 판정.
        //            if (true == weaponHitbox.IsHit(enemyHitbox))
        //            {
        //                _logger.LogWarning($"Hit enemy. pos: {enemy.Location.Pos}");

        //                hitEnemies.Add(enemy);
        //                enemy.Health -= damage;

        //                // 죽음 처리.
        //                if (enemy.Health <= 0)
        //                {
        //                    _logger.LogWarning($"Death enemy.");

        //                    // Death enemy.
        //                    deathEnemies.Add(enemy);
        //                }
        //            }
        //        }
        //    }

        //    // 적 유닛 죽음 처리.
        //    foreach (var enemy in deathEnemies)
        //    {
        //        _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var entity);
        //        _collisionService.Release(entity.UnitCollision);
        //    }
        //}
    }
}
