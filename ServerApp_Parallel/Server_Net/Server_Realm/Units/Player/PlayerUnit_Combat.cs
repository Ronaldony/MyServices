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
        public void AttackMelee(Vector2 angle, IEnumerable<Unit> units)
        {
            var collisionType = EquipmentHandler.GetWeaponCollisionType();
            var damage = EquipmentHandler.GetDamage() + StatHandler.GetPower();
            
            // 피격 적 유닛 탐색.
            var collision = _collisionService.Rent(collisionType).Update(MovementHandler.NowPos(), angle);
            var hitbox = collision.GetHitbox();

            // 유닛 공격.
            foreach (var unit in units)
            {
                unit.CombatHandler.Hit(hitbox, damage);
            }

            // 공격 유닛 탐색.
            _collisionService.Release(collision);
        }
    }
}
