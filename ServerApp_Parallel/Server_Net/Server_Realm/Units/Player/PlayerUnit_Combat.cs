using System.Numerics;

namespace Server_Realm.Entities
{
    using Server_Realm.GameEvents;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Attack melee.
        /// </summary>
        public List<Unit> AttackMelee(Vector2 angle, IEnumerable<Unit> units)
        {
            var collisionType = EquipmentHandler.GetWeaponCollisionType();
            var damage = EquipmentHandler.GetDamage() + StatHandler.GetPower();

            // 공격 이벤트 전달.
            ZoneRouter.RouteGameEvent(new AttackEvent
            {
                Damage = damage,
                RouteSection = MovementHandler.NowSection(),
                Angle = angle,
                AttackerPos = MovementHandler.NowPos(),
                WeaponCollisionType = collisionType
            });

            // 피격 적 유닛 탐색.
            var hitEnemies = new List<Unit>();
            var collision = _collisionService.Rent(collisionType)
                .Update(MovementHandler.NowPos(), angle);
            var hitbox = collision.GetHitbox();

            // 유닛 공격.
            foreach (var unit in units)
            {
                if (unit.CombatHandler.Attack(hitbox, damage))
                {
                    hitEnemies.Add(unit);
                }
            }

            // 공격 유닛 탐색.
            _collisionService.Release(collision);

            return hitEnemies;
        }
    }
}
