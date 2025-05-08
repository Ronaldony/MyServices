using GameCore;
using System.Numerics;

namespace Server_World.Core.Collision
{
    /// <summary>
    /// SwordCollisionBox.
    /// </summary>
    public sealed class SwordCollisionBox : CollisionBox
    {
        public SwordCollisionBox()
        {
            _boudingBox = new BoudingBox();
        }

        /// <summary>
        /// 충돌 관련 값 셋업.
        /// </summary>
        public override void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY)
        {
            _collisionType = collisionType;
            _width = width;
            _height = height / 2;
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        /// <param name="angle">회전 축</param>
        public override BoudingBox GetBoudingBox(float x, float y, Vector2 angle)
        {
            var min = new Vector2();
            var max = new Vector2();

            // X 계산.
            if (angle.X < 0)
            {
                // 왼쪽.
                // 최솟값 = 현재 좌표 + offset - 충돌 범위.
                max.X = x - _offsetX;
                min.X = x - (_offsetX + _width);
            }
            else
            {
                // 오른쪽.
                // 최솟값 = 현재 좌표 + offset - 충돌 범위.
                min.X = x + _offsetX;
                max.X = x + _offsetX + _width;
            }

            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            min.Y = y + _offsetY - _height;
            max.Y = y + _offsetY + _height;

            _boudingBox.Min = min;
            _boudingBox.Max = max;

            return _boudingBox;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        public override BoudingBox GetBoudingBox(float x, float y)
        {
            throw new NotImplementedException();
        }
    }
}
