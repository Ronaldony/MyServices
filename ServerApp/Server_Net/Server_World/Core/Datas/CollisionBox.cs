using System.Numerics;

namespace Server_World.Core.Datas
{
    /// <summary>
    /// CollisionBox.
    /// </summary>
    public class CollisionBox
    {
        // 충돌 너비 및 높이.
        private float _halfWidth, _halfHeight;

        // 충돌 위치 오프셋. (객체 위치로 부터의 거리)
        private float _offsetX, _offsetY;

        private BoudingBox _boudingBox;

        public CollisionBox()
        {
            _boudingBox = new BoudingBox();
        }

        /// <summary>
        /// 충돌 관련 값 셋업.
        /// </summary>
        public void Setup(float width, float height, float offsetX, float offsetY)
        {
            _halfWidth = width / 2;
            _halfHeight = height / 2;
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        /// <summary>
        /// 충돌 경계 박스.
        /// </summary>c
        /// <param name="x">객체 좌표 x</param>
        /// <param name="y">객체 좌표 y</param>
        /// <param name="angle">회전 축</param>
        public BoudingBox GetBoudingBox(float x, float y, Vector2 angle)
        {
            var min = _boudingBox.Min;
            var max = _boudingBox.Max;

            // 최솟값 = 현재 좌표 + offset - 충돌 범위.
            min.X = x + _offsetX - _halfWidth;
            min.Y = y + _offsetY - _halfHeight;

            // 최댓값 = 현재 좌표 + offset + 충돌 범위.
            max.X = x + _offsetX + _halfWidth;
            max.Y = y + _offsetY + _halfHeight;

            return _boudingBox;
        }
    }
}
