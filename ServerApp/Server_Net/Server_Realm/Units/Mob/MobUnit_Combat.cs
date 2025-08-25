namespace Server_Realm.Units.Mob
{
    using GameCore;
    using GameCore.Datas.Projectiles;
    using GameCore.GamePacket;
    using Server_Realm.Entities;

    /// <summary>
    /// MobUnit.
    /// </summary>
    public partial class MobUnit : Unit
    {
        /// <summary>
        /// Is detect player.
        /// </summary>
        public bool IsDetectPlayer(long srcUnitId)
        {
            if (null == WorldSection.DetectPlayerAround(srcUnitId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Detect player.
        /// </summary>
        public PlayerUnit DetectPlayer(long srcUnitId)
        {
            return WorldSection.DetectPlayerAround(srcUnitId);
        }

        /// <summary>
        /// Attack.
        /// </summary>
        public void Attack(Unit target)
        {
            // 원거리 공격.
            switch (Mob.AttackType)
            {
                // 원거리 공격.
                case Type_MobAttack.RasorBall:
                    AttackRange(Type_Projectile.RasorBall, target);
                    break;
                case Type_MobAttack.Grape:
                    AttackRange(Type_Projectile.Grape, target);
                    break;
            }
        }

        /// <summary>
        /// 원거리 공격.
        /// </summary>
        private void AttackRange(Type_Projectile projectileType, Unit target)
        {
            var spawnPos = MovementHandler.NowPos();
            //var angle = Vector2.Normalize(target.MovementHandler.NowPos() - this.MovementHandler.NowPos());

            // 투사체 삽입.
            CombatHandler.OwnProjectile(projectileType, spawnPos, target.MovementHandler.NowPos(), this.StatHandler.GetPower());

            // 투사체 공격.
            var gamePacket = new GamePacketSC_AttackProjectile();

            gamePacket.UnitId = this.UnitId;
            gamePacket.Projectile = new DTO_Projectile
            {
                SpawnPos = spawnPos,
                TargetPos = target.MovementHandler.NowPos(),
                ProjectileType = projectileType
            };

            // 패킷 전송 - 투사체.
            WorldSection.SendPacket_AroundSections(GamePacketCode.SC_AttackProjectile, gamePacket);
        }
    }
}
