using System.Numerics;

namespace Server_Realm.Collisions
{
    using GameCore;
    using Server_Realm.HitBoxes;

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
        public override Collision Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY)
        {
            _collisionType = collisionType;
            Width = width / 2;
            Height = height / 2;
            Offset = new Vector2(offsetX, offsetY);

            return this;
        }

        /// <summary>
        /// 위치 업데이트 - 방향성이 있는 충돌 처리.
        /// </summary>
        public override Collision Update(Vector2 pos, Vector2 angle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 위치 업데이트 - 방향성이 없는 충돌 처리.
        /// </summary>
        public override Collision Update(Vector2 pos)
        {
            // 최솟값 = 현재 좌표 + offset - 충돌 범위.
            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            _hitbox.Setup(
                min: new Vector2(pos.X + Offset.X - Width, pos.Y + Offset.Y - Height),
                max: new Vector2(pos.X + Offset.Y + Width, pos.Y + Offset.Y + Height));

            return this;
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
