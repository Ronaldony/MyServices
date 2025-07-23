using System.Numerics;

namespace Server_World.Core.Collisions
{
    using GameCore;
    using Server_World.Core.HitBoxes;

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
            Offset = new Vector2(offsetX, offsetY);
        }

        /// <summary>
        /// 위치 업데이트.
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="angle">회전 축</param>
        public override void Update(Vector2 pos, Vector2 angle)
        {
            // X 계산.
            // 최솟값 = 현재 좌표 + offset - 충돌 범위.
            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            if (angle.X < 0)
            {
                // 왼쪽.
                _hitbox.Max = new Vector2(pos.X - Offset.X, pos.Y + Offset.Y + Height);
                _hitbox.Min = new Vector2(pos.X - (Offset.X + Width), pos.Y + Offset.Y - Height);
            }
            else
            {
                // 오른쪽.
                _hitbox.Max = new Vector2(pos.X + Offset.X + Width, pos.Y + Offset.Y + Height);
                _hitbox.Min = new Vector2(pos.X + Offset.X, pos.Y + Offset.Y - Height);
            }
        }

        /// <summary>
        /// 위치 업데이트.
        /// </summary>
        public override void Update(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 히트박스.
        /// </summary>
        public override IHitbox GetHitbox()
        {
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
