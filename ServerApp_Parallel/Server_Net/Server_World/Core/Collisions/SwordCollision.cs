using System.Numerics;

namespace Server_World.Core.Collisions
{
    using GameCore;
    using Server_World.Core.Collisions.Boundings;

    /// <summary>
    /// SwordCollision.
    /// </summary>
    public sealed class SwordCollision : Collision
    {
        /// <summary>
        /// SquareHitbox.
        /// </summary>
        private readonly SquareHitbox _hitbox;

        public SwordCollision()
        {
            _hitbox = new SquareHitbox();
        }

        /// <summary>
        /// 충돌 관련 값 셋업.
        /// </summary>
        public override void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY)
        {
            _collisionType = collisionType;
            Width = width;
            Height = height / 2;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        /// <param name="angle">회전 축</param>
        public override Hitbox GetHitbox(float x, float y, Vector2 angle)
        {
            // X 계산.
            if (angle.X < 0)
            {
                // 왼쪽.
                // 최솟값 = 현재 좌표 + offset - 충돌 범위.
                _hitbox.MaxX = x - OffsetX;
                _hitbox.MinX = x - (OffsetX + Width);
            }
            else
            {
                // 오른쪽.
                // 최솟값 = 현재 좌표 + offset - 충돌 범위.
                _hitbox.MinX = x + OffsetX;
                _hitbox.MaxX = x + OffsetX + Width;
            }

            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            _hitbox.MinY = y + OffsetY - Height;
            _hitbox.MaxY = y + OffsetY + Height;

            return _hitbox;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        public override Hitbox GetHitBox(float x, float y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
