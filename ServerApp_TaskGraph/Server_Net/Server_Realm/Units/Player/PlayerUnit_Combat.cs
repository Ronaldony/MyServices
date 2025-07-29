using GameCore;
using System.Numerics;

namespace Server_Realm.Entities
{
    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Attack melee.
        /// </summary>
        public void AttackMelee(Vector2 angle, ICollection<Unit> units)
        {
            var collisionType = EquipmentHandler.GetWeaponCollisionType();
            var damage = EquipmentHandler.GetDamage() + StatHandler.GetPower();
            
            // 피격 적 유닛 탐색.
            var collision = _collisionService.Create(collisionType).Update(MovementHandler.NowPos(), angle);
            var hitbox = collision.GetHitbox();

            // 유닛 공격.
            foreach (var unit in units)
            {
                unit.CombatHandler.Hit(hitbox, damage);
            }

            // 공격 유닛 탐색.
            _collisionService.Return(collision);
        }

        /// <summary>
        /// Create projectile.
        /// </summary>
        public void CreateProjectile(Vector2 spawnPos, Vector2 angle)
        {
            var projectileType = EquipmentHandler.GetProjectileType();

            if (projectileType == Type_Projectile.None)
            {
                return;
            }

            // 투사체 삽입.
            CombatHandler.OwnProjectile(projectileType, spawnPos, angle, EquipmentHandler.GetDamage());
        }
    }
}
