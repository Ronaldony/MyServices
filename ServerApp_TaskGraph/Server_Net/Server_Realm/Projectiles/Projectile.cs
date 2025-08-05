using System.Numerics;

namespace Server_Realm.Projectiles
{
    using GameCore;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;
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
        /// 섹션.
        /// </summary>
        public SectionId Section { get; protected set; }

        /// <summary>
        /// Damage.
        /// </summary>
        public int Damage { get; protected set; }

        /// <summary>
        /// 투사체 생성 위치. 
        /// </summary>
        protected Vector2 SpawnPos;

        /// <summary>
        /// 현재 위치. 
        /// </summary>
        protected Vector2 Pos;

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
        public abstract void Setup(int projectileId, Unit owner, Vector2 spawnPos, Vector2 angle, int damage, int radius, int velocity, int maxRange, bool isPenetrates, bool isGrenade);

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
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
