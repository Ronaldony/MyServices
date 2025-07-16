using System.Numerics;

namespace Server_World.Core.Collisions
{
    using GameCore;
    using Server_World.Core.Collisions.Boundings;

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
        protected float OffsetX;
        protected float OffsetY;

        /// <summary>
        /// Set up.
        /// </summary>
        public abstract void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY);

        /// <summary>
        /// 바운딩 경계 얻기.
        /// </summary>
        public abstract Hitbox GetHitbox(float x, float y, Vector2 angle);

        /// <summary>
        /// 바운딩 경계 얻기.
        /// </summary>
        public abstract Hitbox GetHitBox(float x, float y);

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
