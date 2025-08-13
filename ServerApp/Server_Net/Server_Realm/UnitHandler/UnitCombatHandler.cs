using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_Realm.UnitHandler
{
    using GameCore;
    using Server_Realm.Core.Factories;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;
    using Server_Realm.Projectiles;

    /// <summary>
    /// UnitCombatHandler.
    /// </summary>
    public class UnitCombatHandler : UnitHandlerBase
    {
        private readonly ILogger<UnitCombatHandler> _logger;

        // Projectile factory.
        private readonly ProjectileFactory _projectileFactory;
        private readonly CollisionFactory _collisionFactory;

        // Unit.
        private Unit _owner;

        // Projectiles
        private readonly HashSet<Projectile> _projectiles;
        private readonly HashSet<Projectile> _grenades;
        private readonly List<Projectile> _distroyProjectiles;

        public UnitCombatHandler(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitCombatHandler>>();

            _projectileFactory = serviceProvider.GetRequiredService<ProjectileFactory>();
            _collisionFactory = serviceProvider.GetRequiredService<CollisionFactory>();

            _projectiles = new HashSet<Projectile>();
            _grenades = new HashSet<Projectile>();
            _distroyProjectiles = new List<Projectile>();
        }

        /// <summary>
        /// 기본 상태 초기화.
        /// </summary>
        public void Setup(Unit owner)
        {
            _owner = owner;
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

                _logger.LogDebug($"projectile pos: {prjt.Pos}");

                if (prjt.IsMaxRange())
                {
                    _distroyProjectiles.Add(prjt);
                }
            }

            // 유탄 처리.
            foreach (var grenade in _grenades)
            {
                grenade.MoveNextFrame(deltaTime);

                _logger.LogDebug($"grenade pos: {grenade.Pos}");

                if (grenade.IsMaxRange())
                {
                    _distroyProjectiles.Add(grenade);
                }
            }
        }

        /// <summary>
        /// Post update.
        /// </summary>
        public void PostUpdate()
        {
            // Distroy Projectiles.
            DistroyProjectiles();
        }

        /// <summary>
        /// 피격.
        /// </summary>
        public void Hit(IHitbox srcHitbox, int damage)
        {
            // Hit box 충돌 검사.
            if (true == srcHitbox.IsHit(_owner.UnitCollision.GetHitbox()))
            {
                _owner.StatHandler.Damage(damage);
            }
        }

        #region melee
        /// <summary>
        /// 유닛과 근접 공격 처리.
        /// </summary>
        public void CollideMelee(Type_Collision collisionType, Vector2 angle, int damage, IEnumerable<Unit> units)
        {
            // 피격 적 유닛 탐색.
            var collision = _collisionFactory.Create(collisionType).Update(_owner.MovementHandler.NowPos(), angle);
            var hitbox = collision.GetHitbox();

            // 히트 판정.
            foreach (var unit in units)
            {
                unit.CombatHandler.Hit(hitbox, damage);
            }

            // Release.
            _collisionFactory.Return(collision);
        }
        #endregion

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
        /// 유닛과 투사체 충돌.
        /// </summary>
        public void CollideGrenade(IEnumerable<Unit> units)
        {
            // 유닛-투사체 충돌 검사.
            foreach (var grenade in _grenades)
            {
                if (grenade.IsMaxRange())
                {
                    foreach (var unit in units)
                    {
                        unit.CombatHandler.Hit(grenade.GetHitbox(), grenade.Damage);
                    }
                }
            }
        }

        /// <summary>
        /// Projectile.
        /// </summary>
        public void OwnProjectile(Type_Projectile projectileType, Vector2 spawnPos, Vector2 angle, int damage)
        {
            var prjt = _projectileFactory.Create(_owner, projectileType, spawnPos, angle, damage);

            // 유탄과 일반 투사체를 구분. (충돌 처리 구별)
            if (prjt.IsGrenade)
            {
                // 유탄.
                _grenades.Add(prjt);
            }
            else 
            {
                // 일반 투사체.
                _projectiles.Add(prjt);
            }
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
                _grenades.Remove(prjt);
                _projectileFactory.Return(prjt);
            }

            _distroyProjectiles.Clear();
        }
        #endregion

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            // Projectile.
            foreach (var prjt in _projectiles)
            {
                _projectileFactory.Return(prjt);
            }

            // Grenade.
            foreach (var grenade in _grenades)
            {
                _projectileFactory.Return(grenade);
            }
        }
    }
}
