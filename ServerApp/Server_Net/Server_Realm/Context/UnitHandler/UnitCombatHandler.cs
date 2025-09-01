using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_Realm.Context.UnitHandler
{
    using GameCore;
    using Server_Realm.Core.Factories;
    using Server_Realm.Context.HitBoxes;
    using Server_Realm.Context.Projectiles;
    using Server_Realm.Context.Units;

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

        /// <summary>
        /// Aggro units.
        /// key: Unit.
        /// value: aggro point.
        /// </summary>
        private readonly Dictionary<Unit, int> _aggroPoints;

        public UnitCombatHandler(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitCombatHandler>>();

            _projectileFactory = serviceProvider.GetRequiredService<ProjectileFactory>();
            _collisionFactory = serviceProvider.GetRequiredService<CollisionFactory>();

            _projectiles = new HashSet<Projectile>();
            _grenades = new HashSet<Projectile>();
            _distroyProjectiles = new List<Projectile>();

            _aggroPoints = new Dictionary<Unit, int>();
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
                if (prjt.IsActive)
                {
                    prjt.MoveNextFrame(deltaTime);

                    // 최대 이동거리 검사.
                    if (prjt.IsMaxRange())
                    {
                        _distroyProjectiles.Add(prjt);
                    }
                }
                else
                {
                    // 비활성화 투사체.
                    _distroyProjectiles.Add(prjt);
                }
            }

            // 유탄 처리.
            foreach (var grenade in _grenades)
            {
                if (grenade.IsActive)
                {
                    grenade.MoveNextFrame(deltaTime);

                    // 최대 이동거리 검사.
                    if (grenade.IsMaxRange())
                    {
                        _distroyProjectiles.Add(grenade);
                    }
                }
                else
                {
                    // 비활성화 투사체.
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
        public bool Hit(Unit aggroUnit, IHitbox srcHitbox, int damage)
        {
            // Hit box 충돌 검사.
            if (true == srcHitbox.IsHit(_owner.UnitCollision.GetHitbox()))
            {
                _logger.LogDebug($"Hit unit. unit type: {_owner.UnitType}");

                var hitDamage = _owner.StatHandler.Damage(damage);

                // Aggro 유닛 데미지 축적.
                if (false == _aggroPoints.ContainsKey(aggroUnit))
                {
                    _aggroPoints.Add(aggroUnit, 0);
                }

                _aggroPoints[aggroUnit] += hitDamage;

                return true;
            }

            return false;
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
                unit.CombatHandler.Hit(_owner, hitbox, damage);
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
                if (false == prjt.IsActive)
                {
                    continue;
                }

                foreach (var unit in units)
                {
                    // 피격 판정인 경우, 관통 여부 검사.
                    if (unit.CombatHandler.Hit(_owner, prjt.GetHitbox(), prjt.Damage))
                    {
                        // 관통이 아닌 경우, 투사체 비활성화.
                        if (false == prjt.IsPenetrates)
                        {
                            prjt.InActiveProjectile();
                            break;
                        }
                    }
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
                        unit.CombatHandler.Hit(_owner, grenade.GetHitbox(), grenade.Damage);
                    }
                }
            }
        }

        /// <summary>
        /// Projectile.
        /// </summary>
        public void OwnProjectile(Type_Projectile projectileType, Vector2 spawnPos, Vector2 endPos, int damage)
        {
            var prjt = _projectileFactory.Create(_owner, projectileType, spawnPos, endPos, damage);

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

        #region aggro
        /// <summary>
        /// Aggro 유닛 초기화.
        /// </summary>
        public void ClearAggroPoints()
        {
            _aggroPoints.Clear();
        }

        /// <summary>
        /// Aggro 포인트 반환.
        /// </summary>
        public Dictionary<Unit, int> GetAggroPoints()
        {
            return _aggroPoints;
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
