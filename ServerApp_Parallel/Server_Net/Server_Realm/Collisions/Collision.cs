using System.Numerics;

namespace Server_Realm.Collisions
{
    using GameCore;
    using Server_Realm.HitBoxes;

    /// <summary>
    /// Collision.
    /// </summary>
    public abstract class Collision : IDisposable
    {
        /// <summary>
        /// Type_Collision.
        /// </summary>
        public Type_Collision CollisionType => _collisionType;
        protected Type_Collision _collisionType;

        /// <summary>
        /// 충돌 너비 및 높이. 
        /// </summary>
        protected float Width;
        protected float Height;

        /// <summary>
        /// 충돌 위치 오프셋. (객체 위치로 부터의 거리)
        /// </summary>
        protected Vector2 Offset;

        /// <summary>
        /// 셋업.
        /// </summary>
        public abstract void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY);

        /// <summary>
        /// 위치 업데이트.
        /// </summary>
        public abstract void Update(Vector2 pos, Vector2 angle);

        /// <summary>
        /// 위치 업데이트.
        /// </summary>
        public abstract void Update(Vector2 pos);

        /// <summary>
        /// 히트박스.
        /// </summary>
        public abstract IHitbox GetHitbox();

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
