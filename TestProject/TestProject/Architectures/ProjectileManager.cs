using System.Collections.Concurrent;
using System.Numerics;
using TestProject.Datas;

namespace TestProject.Architectures
{
    /// <summary>
    /// 서버 측 발사체 관리자
    /// </summary>
    public class ProjectileManager
    {
        private readonly ConcurrentDictionary<int, Projectile> activeProjectiles;
        public IDictionary<int, Projectile> ActiveProjectiles => activeProjectiles;
        private readonly SpatialHashGrid spatialHash;
        private readonly ObjectPool<Projectile> projectilePool;
        private readonly IGameWorld gameWorld;
        private int nextProjectileId = 1;

        // 성능 통계
        public int ActiveProjectileCount => activeProjectiles.Count;

        public float LastUpdateTime { get; private set; }

        public ProjectileManager(IGameWorld world, float spatialHashCellSize = 10f)
        {
            gameWorld = world;
            activeProjectiles = new ConcurrentDictionary<int, Projectile>();
            spatialHash = new SpatialHashGrid(spatialHashCellSize);
            projectilePool = new ObjectPool<Projectile>(() => new Projectile(Vector3.Zero, Vector3.Zero, 0, 0), 1000);
        }

        /// <summary>
        /// 발사체 생성 (서버 권위)
        /// </summary>
        public int CreateProjectile(ProjectileData data)
        {
            // 발사 각도 계산
            var angles = ProjectileTrajectory.CalculateLaunchAngle(
                data.Origin, data.Target, data.LaunchSpeed);

            if (!angles.lowAngle.HasValue)
            {
                return -1; // 목표 도달 불가
            }

            // 발사 각도 선택 (저각/고각)
            float angle = data.UseHighAngle ? angles.highAngle.Value : angles.lowAngle.Value;

            // 발사 속도 계산
            Vector3 velocity = ProjectileTrajectory.CalculateLaunchVelocity(
                data.Origin, data.Target, angle, data.LaunchSpeed);

            // 풀에서 발사체 가져오기
            var projectile = projectilePool.Get();
            projectile.Id = nextProjectileId++;
            projectile.OwnerId = data.OwnerId;
            projectile.Position = data.Origin;
            projectile.StartPosition = data.Origin;
            projectile.Velocity = velocity;
            projectile.Damage = data.Damage;
            projectile.Radius = data.Radius;
            projectile.MaxRange = data.MaxRange;
            projectile.IsPiercing = data.IsPiercing;
            projectile.MaxPierceCount = data.MaxPierceCount;
            projectile.LifeTime = 0;
            projectile.IsActive = true;
            projectile.HitEntities.Clear();

            activeProjectiles[projectile.Id] = projectile;

            // 클라이언트에게 발사체 생성 알림
            BroadcastProjectileCreated(projectile);

            return projectile.Id;
        }

        /// <summary>
        /// 모든 발사체 업데이트 (서버 틱마다 호출)
        /// </summary>
        public void UpdateProjectiles(float deltaTime)
        {
            var startTime = DateTime.Now;

            // 공간 해시 재구성
            spatialHash.Clear();

            // 활성 엔티티들을 공간 해시에 추가
            foreach (var entity in gameWorld.GetActiveEntities())
            {
                spatialHash.Insert(entity);
            }

            // 병렬 발사체 업데이트
            var projectilesToRemove = new List<int>();

            Parallel.ForEach(activeProjectiles.Values, projectile =>
            {
                if (!projectile.IsActive)
                {
                    lock (projectilesToRemove)
                    {
                        projectilesToRemove.Add(projectile.Id);
                    }
                    return;
                }

                UpdateSingleProjectile(projectile, deltaTime);
            });

            // 비활성 발사체 제거
            foreach (var id in projectilesToRemove)
            {
                if (activeProjectiles.TryRemove(id, out var projectile))
                {
                    projectilePool.Return(projectile);
                    BroadcastProjectileDestroyed(id);
                }
            }

            LastUpdateTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 단일 발사체 업데이트 및 충돌 처리
        /// </summary>
        private void UpdateSingleProjectile(Projectile projectile, float deltaTime)
        {
            // 활성 투사체가 아닌 경우 업데이트 무시.
            if (!projectile.IsActive) return;

            var previousPos = projectile.Position;
            projectile.Update(deltaTime);

            // 연속 충돌 감지 (CCD)
            var hits = spatialHash.Raycast(
                previousPos,
                projectile.Position,
                projectile.Radius
            );

            int pierceCount = 0;
            foreach (var hit in hits)
            {
                // 이미 맞은 대상 제외
                if (projectile.HitEntities.Contains(hit.Object.Id))
                    continue;

                // 자기 자신 제외
                if (hit.Object.Id == projectile.OwnerId)
                    continue;

                // 충돌 처리
                HandleProjectileHit(projectile, hit.Object, hit.Point);
                projectile.HitEntities.Add(hit.Object.Id);

                pierceCount++;

                // 관통 제한
                if (!projectile.IsPiercing || pierceCount >= projectile.MaxPierceCount)
                {
                    projectile.IsActive = false;
                    break;
                }

                // 관통 시 데미지 감소
                projectile.Damage *= 1f - projectile.DamageReductionPerPierce;
            }
        }

        /// <summary>
        /// 발사체 충돌 처리
        /// </summary>
        private void HandleProjectileHit(Projectile projectile, ICollidable target, Vector3 hitPoint)
        {
            // 데미지 적용
            if (target is IDamageable damageable)
            {
                var damageInfo = new DamageInfo
                {
                    AttackerId = projectile.OwnerId,
                    Damage = projectile.Damage,
                    Position = hitPoint,
                    IsCritical = CalculateCritical(projectile),
                    DamageType = Type_Damage.Projectile
                };

                damageable.TakeDamage(damageInfo);
            }

            // 충돌 이펙트 생성
            BroadcastProjectileHit(projectile.Id, target.Id, hitPoint);

            // 충돌 통계 기록
            gameWorld.RecordHit(projectile.OwnerId, target.Id, projectile.Damage);
        }

        /// <summary>
        /// AOE 발사체 폭발 처리
        /// </summary>
        public void ExplodeProjectile(int projectileId, float explosionRadius)
        {
            if (!activeProjectiles.TryGetValue(projectileId, out var projectile))
                return;

            // 폭발 범위 내 모든 대상 검색
            var explosionBounds = new BoundingBox(projectile.Position, explosionRadius);
            var targets = spatialHash.Query(new ExplosionQuery(explosionBounds));

            foreach (var target in targets)
            {
                float distance = Vector3.Distance(projectile.Position, target.Position);
                if (distance <= explosionRadius)
                {
                    // 거리에 따른 데미지 감소
                    float damageMultiplier = 1f - distance / explosionRadius;
                    float explosionDamage = projectile.Damage * damageMultiplier;

                    if (target is IDamageable damageable)
                    {
                        damageable.TakeDamage(new DamageInfo
                        {
                            AttackerId = projectile.OwnerId,
                            Damage = explosionDamage,
                            Position = target.Position,
                            DamageType = Type_Damage.Explosion
                        });
                    }
                }
            }

            projectile.IsActive = false;
            BroadcastExplosion(projectile.Position, explosionRadius);
        }

        /// <summary>
        /// 클라이언트 예측 검증
        /// </summary>
        public bool ValidateClientPrediction(int projectileId, Vector3 clientPosition, float tolerance = 1f)
        {
            if (!activeProjectiles.TryGetValue(projectileId, out var projectile))
                return false;

            float distance = Vector3.Distance(projectile.Position, clientPosition);
            return distance <= tolerance;
        }

        private bool CalculateCritical(Projectile projectile)
        {
            // 크리티컬 확률 계산 로직
            return Random.Shared.NextDouble() < 0.1; // 10% 크리티컬 확률
        }

        private void BroadcastProjectileCreated(Projectile projectile)
        {
            // 네트워크 브로드캐스트 구현
        }

        private void BroadcastProjectileDestroyed(int projectileId)
        {
            // 네트워크 브로드캐스트 구현
        }

        private void BroadcastProjectileHit(int projectileId, int targetId, Vector3 hitPoint)
        {
            // 네트워크 브로드캐스트 구현
        }

        private void BroadcastExplosion(Vector3 position, float radius)
        {
            // 네트워크 브로드캐스트 구현
        }
    }

    /// <summary>
    /// 발사체 생성 데이터
    /// </summary>
    public struct ProjectileData
    {
        public int OwnerId { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Target { get; set; }
        public float LaunchSpeed { get; set; }
        public float Damage { get; set; }
        public float Radius { get; set; }
        public float MaxRange { get; set; }
        public bool UseHighAngle { get; set; }
        // 관통 여부
        public bool IsPiercing { get; set; }
        public int MaxPierceCount { get; set; }
    }

    /// <summary>
    /// 오브젝트 풀
    /// </summary>
    public class ObjectPool<T>
    {
        private readonly ConcurrentBag<T> pool;
        private readonly Func<T> objectGenerator;

        public ObjectPool(Func<T> generator, int initialSize)
        {
            objectGenerator = generator;
            pool = new ConcurrentBag<T>();

            for (int i = 0; i < initialSize; i++)
            {
                pool.Add(objectGenerator());
            }
        }

        public T Get()
        {
            return pool.TryTake(out T item) ? item : objectGenerator();
        }

        public void Return(T item)
        {
            pool.Add(item);
        }
    }
}