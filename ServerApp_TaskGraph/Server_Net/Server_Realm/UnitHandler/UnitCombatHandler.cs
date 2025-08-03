using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_Realm.UnitHandler
{
    using GameCore;
    using Server_Realm.Core.Factory;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;
    using Server_Realm.Projectiles;

    /// <summary>
    /// UnitCombatHandler.
    /// </summary>
    public class UnitCombatHandler
    {
        private readonly ILogger<UnitCombatHandler> _logger;

        // Projectile service.
        private readonly ProjectileFactory _projectilefActory;

        // Unit.
        private readonly Unit _owner;

        // Projectiles
        private readonly HashSet<Projectile> _projectiles;
        private readonly List<Projectile> _distroyProjectiles;

        public UnitCombatHandler(IServiceProvider serviceProvider, Unit owner)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitCombatHandler>>();
            
            _projectilefActory = serviceProvider.GetRequiredService<ProjectileFactory>();
            
            _owner = owner;

            _projectiles = new HashSet<Projectile>();
            _distroyProjectiles = new List<Projectile>();
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(double deltaTime)
        {
            // 투사체 이동.
            foreach (var prjt in _projectiles)
            {
                prjt.MoveNextFrame(deltaTime);

                if (!prjt.IsMaxRange())
                {
                    _distroyProjectiles.Add(prjt);
                }
            }

            // Distroy Projectiles.
            DistroyProjectiles();
        }

        /// <summary>
        /// 피격.
        /// </summary>
        public void Hit(IHitbox hitbox, int damage)
        {
            // Hit box 충돌 검사.
            if (true == hitbox.IsHit(_owner.UnitCollision.GetHitbox()))
            {
                _owner.StatHandler.Damage(damage);
            }
        }

        #region projectiles
        /// <summary>
        /// 유닛과 투사체 충돌.
        /// </summary>
        public void CollideProjectiles(IEnumerable<Unit> units)
        {
            // 유닛-투사체 충돌 검사.
            foreach (var prjt in _projectiles)
            {
                foreach (var unit in units)
                {
                    unit.CombatHandler.Hit(prjt.GetHitbox(), prjt.Damage);
                }
            }
        }

        /// <summary>
        /// Projectile.
        /// </summary>
        public void OwnProjectile(Type_Projectile projectileType, Vector2 spawnPos, Vector2 angle, int damage)
        {
            // Add projectile.
            _projectiles.Add(_projectilefActory.Create(_owner, projectileType, spawnPos, angle, damage));
        }

        /// <summary>
        /// Distroy projectile.
        /// </summary>
        public void DistroyProjectiles()
        {
            // Distroy Projectiles.
            foreach (var prjt in _distroyProjectiles)
            {
                _projectiles.Remove(prjt);
                _projectilefActory.Return(prjt);
            }

            _distroyProjectiles.Clear();
        }
        #endregion
    }
}
