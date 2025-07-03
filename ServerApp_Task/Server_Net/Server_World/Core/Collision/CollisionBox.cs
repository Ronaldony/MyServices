using Server_World.Core.Services;
using System.Numerics;

namespace Server_World.Core.Collision
{
    using GameCore;

    /// <summary>
    /// ICollisionBox.
    /// </summary>
    public abstract class CollisionBox : IDisposable
    {
        // 충돌 너비 및 높이.
        protected float _width, _height;

        // 충돌 위치 오프셋. (객체 위치로 부터의 거리)
        protected float _offsetX, _offsetY;

        // 바운딩 경계.
        protected BoudingBox _boudingBox;

        // Collision type.
        public Type_Collision CollisionType => _collisionType;
        protected Type_Collision _collisionType;

        /// <summary>
        /// Set up.
        /// </summary>
        public abstract void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY);

        /// <summary>
        /// 바운딩 경계 얻기.
        /// </summary>
        public abstract BoudingBox GetBoudingBox(float x, float y, Vector2 angle);

        /// <summary>
        /// 바운딩 경계 얻기.
        /// </summary>
        public abstract BoudingBox GetBoudingBox(float x, float y);

        public void Dispose()
        {
        }
    }
}
