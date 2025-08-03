
namespace Server_Realm.Units.Enemy
{
    using GameCore;
    using Server_Realm.Entities;
    using System.Numerics;

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
            // TODO: 로직 개선.
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
                // 근접 공격.
                case Type_MobAttack.Melee:
                    break;
                // 원거리 공격.
                case Type_MobAttack.RasorBall:
                    AttackRange(Type_Projectile.RasorBall, target);
                    break;
                case Type_MobAttack.Grenade:
                    AttackRange(Type_Projectile.Grenade, target);
                    break;
            }
        }

        /// <summary>
        /// 원거리 공격.
        /// </summary>
        private void AttackRange(Type_Projectile projectileType, Unit target)
        {
            var spawnPos = target.MovementHandler.NowPos();
            var angle = Vector2.Normalize(this.MovementHandler.NowPos() - target.MovementHandler.NowPos());

            // 투사체 삽입.
            CombatHandler.OwnProjectile(projectileType, spawnPos, angle, this.StatHandler.GetPower());
        }
    }
}
