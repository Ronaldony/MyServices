using System.Numerics;

namespace Server_Realm.HitBoxes
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// SquareHitbox.
    /// </summary>
    public class SquareHitbox : IHitbox
    {
        public SquareHitbox()
        {
            HitboxType = Type_Hitbox.Square;
        }

        /// <summary>
        /// Type_Hitbox.
        /// </summary>
        public Type_Hitbox HitboxType { get; init; }

        /// <summary>
        /// 위치 정보.
        /// </summary>
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        /// <summary>
        /// Setup hit box.
        /// </summary>
        public void Setup(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Check hit with target hit box.
        /// </summary>
        public bool IsHit(IHitbox target)
        {
            bool isHit = false;

            switch (target.HitboxType)
            {
                case Type_Hitbox.Square:
                    var squareHitbox = target as SquareHitbox;
                    isHit = HitSquare(squareHitbox);
                    break;
                case Type_Hitbox.Sphere:
                    var sphereHitbox = target as SphereHitbox;
                    isHit = HitSphere(sphereHitbox);
                    break;
                default:
                    break;
            }

            return isHit;
        }

        /// <summary>
        /// Hit square.
        /// </summary>
        private bool HitSquare(SquareHitbox target)
        {
            // 가로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (Min.X > target.Max.X || Max.X < target.Min.X)
            {
                return false;
            }

            // 세로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (Min.Y > target.Max.Y || Max.Y < target.Min.Y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Hit sphere.
        /// </summary>
        private bool HitSphere(SphereHitbox target)
        {
            var targetCenter = target.Center;

            // 접점.
            var clampX = Math.Clamp(targetCenter.X, Min.X, Max.X);
            var clampY = Math.Clamp(targetCenter.Y, Min.Y, Max.Y);

            // 거리.
            var dx = clampX - targetCenter.X;
            var dy = clampY - targetCenter.Y;

            // 피타고라스.
            return dx * dx + dy * dy <= target.RadiusSqr;
        }
    }
}
