using System.Numerics;

namespace Server_Realm.Entities
{
    using GameCore;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit
    {
        /// <summary>
        /// Attack melee.
        /// </summary>
        public void AttackMelee(Vector2 angle)
        {
            // 주변 몹 정보.
            var aroundMobs = WorldSection.DetectAroundMobs();

            // 충돌 처리 관련.
            var collisionType = EquipmentHandler.GetWeaponCollisionType();

            // Collide melee.
            CombatHandler.CollideMelee(collisionType, angle, EquipmentHandler.GetDamage(), aroundMobs);
        }

        /// <summary>
        /// Create projectile.
        /// </summary>
        public void CreateProjectile(Vector2 spawnPos, Vector2 endPos)
        {
            var projectileType = EquipmentHandler.GetProjectileType();

            if (projectileType == Type_Projectile.None)
            {
                return;
            }

            // 투사체 삽입.
            CombatHandler.OwnProjectile(projectileType, spawnPos, endPos, EquipmentHandler.GetDamage());
        }
    }
}
