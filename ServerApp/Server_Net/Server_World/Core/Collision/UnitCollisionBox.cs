using GameCore;
using System.Numerics;

namespace Server_World.Core.Collision
{
    /// <summary>
    /// HitCollisionBox.
    /// </summary>
    public class UnitCollisionBox : CollisionBox
    {
        public UnitCollisionBox()
        {
            _boudingBox = new BoudingBox();
        }

        /// <summary>
        /// 충돌 관련 값 셋업.
        /// </summary>
        public override void Setup(Type_Collision collisionType, float width, float height, float offsetX, float offsetY)
        {
            _collisionType = collisionType;
            _width = width / 2;
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
            // TODO: Angle에 따른 좌표 계산.
            if (angle.X < 0)
            {
                // 왼쪽.
                // 최솟값 = 현재 좌표 + offset - 충돌 범위.
                _boudingBox.MinX = x - (_offsetX + _width);
                _boudingBox.MaxX = x - _offsetX + _width;
            }
            else
            {
                // 오른쪽.
                // 최솟값 = 현재 좌표 + offset - 충돌 범위.
                _boudingBox.MinX = x + _offsetX - _width;
                _boudingBox.MaxX = x + _offsetX + _width;
            }

            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            _boudingBox.MinY = y + _offsetY - _height;
            _boudingBox.MaxY = y + _offsetY + _height;

            return _boudingBox;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        public override BoudingBox GetBoudingBox(float x, float y)
        {
            // 최솟값 = 현재 좌표 + offset - 충돌 범위.
            _boudingBox.MinX = x + _offsetX - _width;
            _boudingBox.MinY = y + _offsetY - _height;

            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            _boudingBox.MaxX = x + _offsetX + _width;
            _boudingBox.MaxY = y + _offsetY + _height;

            return _boudingBox;
        }
    }
}
