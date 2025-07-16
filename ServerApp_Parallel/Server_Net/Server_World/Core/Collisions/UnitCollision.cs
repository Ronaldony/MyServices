using System.Numerics;

namespace Server_World.Core.Collisions
{
    using GameCore;
    using Server_World.Core.Collisions.Boundings;

    /// <summary>
    /// HitCollisionBox.
    /// </summary>
    public class UnitCollision : Collision
    {
        /// <summary>
        /// SquareHitbox.
        /// </summary>
        private readonly SquareHitbox _hitbox;

        public UnitCollision()
        {
            _hitbox = new SquareHitbox();
        }

        /// <summary>
        /// 충돌 관련 값 셋업.
        /// </summary>
        public override void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY)
        {
            _collisionType = collisionType;
            Width = width / 2;
            Height = height / 2;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override Hitbox GetHitbox(float x, float y, Vector2 angle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        public override Hitbox GetHitBox(float x, float y)
        {
            // 최솟값 = 현재 좌표 + offset - 충돌 범위.
            _hitbox.MinX = x + OffsetX - Width;
            _hitbox.MinY = y + OffsetY - Height;

            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            _hitbox.MaxX = x + OffsetX + Width;
            _hitbox.MaxY = y + OffsetY + Height;

            return _hitbox;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
