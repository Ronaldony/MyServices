using System.Numerics;

namespace Server_Realm.Context.Projectiles
{
    using GameCore;
    using Server_Realm.Context.HitBoxes;
    using Server_Realm.Context.Units;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Worlds;

    /// <summary>
    /// Projectile.
    /// </summary>
    public abstract class Projectile : IDisposable
    {
        /// <summary>
        /// Type_Projectile.
        /// </summary>
        public Type_Projectile ProjectilType => _projectileType;
        private readonly Type_Projectile _projectileType;

        /// <summary>
        /// 투사체 Id.
        /// </summary>
        public int ProjectileId { get; protected set; }

        /// <summary>
        /// Owner.
        /// </summary>
        public Unit Owner { get; protected set; }

        /// <summary>
        /// 관통 여부.
        /// </summary>
        public bool IsPenetrates { get; protected set; }

        /// <summary>
        /// 유탄 여부.
        /// </summary>
        public bool IsGrenade { get; protected set; }

        /// <summary>
        /// 투사체 활성 여부.
        /// </summary>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// 섹션.
        /// </summary>
        public SectionId Section { get; protected set; }

        /// <summary>
        /// Damage.
        /// </summary>
        public int Damage { get; protected set; }

        /// <summary>
        /// 현재 위치. 
        /// </summary>
        public Vector2 Pos { get; protected set; }

        /// <summary>
        /// 투사체 생성 위치. 
        /// </summary>
        protected Vector2 SpawnPos;

        /// <summary>
        /// 투사체 타겟 위치. 
        /// </summary>
        protected Vector2 EndPos;

        /// <summary>
        /// 투사 각도. 
        /// </summary>
        protected Vector2 Angle;

        /// <summary>
        /// 속도. 
        /// </summary>
        protected int Velocity;

        /// <summary>
        /// 최대 이동 거리.
        /// </summary>
        protected int MaxRange;

        /// <summary>
        /// 최대 이동 거리 제곱근. (거리 비교용)
        /// </summary>
        protected int MaxRangeSqr;

        public Projectile(Type_Projectile projectile)
        {
            _projectileType = projectile;
        }

        /// <summary>
        /// Setup.
        /// </summary>
        public virtual void Setup(int projectileId, Unit owner, Vector2 spawnPos, Vector2 endPos, int damage, TB_Projectile tbProjectile)
        {
            ProjectileId = projectileId;
            Owner = owner;

            SpawnPos = spawnPos;
            Pos = spawnPos;
            EndPos = endPos;
            Angle = VectorUtility.SafeNormalize(endPos, spawnPos);
            Section = SectionId.ToSection(spawnPos);

            Damage = damage;
            Velocity = tbProjectile.Velocity;
            MaxRange = tbProjectile.MaxRange;
            MaxRangeSqr = MaxRange * MaxRange;

            IsPenetrates = tbProjectile.IsPenetrates;
            IsGrenade = tbProjectile.IsGrenade;
            IsActive = true;
        }

        /// <summary>
        /// Move for next frame.
        /// </summary>
        public abstract void MoveNextFrame(double deltaTime);

        /// <summary>
        /// Check move to max range or not.
        /// </summary>
        public abstract bool IsMaxRange();

        /// <summary>
        /// Get hit box.
        /// </summary>
        /// <returns></returns>
        public abstract IHitbox GetHitbox();

        /// <summary>
        /// Inactivate projectile.
        /// </summary>
        public void InActiveProjectile()
        {
            IsActive = false;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
