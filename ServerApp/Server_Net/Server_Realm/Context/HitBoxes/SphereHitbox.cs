
using System.Numerics;

namespace Server_Realm.Context.HitBoxes
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// SphereHitbox.
    /// </summary>
    public class SphereHitbox : IHitbox
    {
        public SphereHitbox()
        {
            HitboxType = Type_Hitbox.Sphere;
        }

        /// <summary>
        /// Type_Hitbox.
        /// </summary>
        public Type_Hitbox HitboxType { get; init; }

        /// <summary>
        /// 반지름.
        /// </summary>
        public float Radius;

        /// <summary>
        /// 반지름 제곱.
        /// </summary>
        public float RadiusSqr;

        /// <summary>
        /// 중심 위치.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Setup hit box.
        /// </summary>
        /// <param name="radius"></param>
        public void Setup(float radius, Vector2 center)
        {
            Center = center;
            Radius = radius;
            RadiusSqr = radius * radius;
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
            // 접점.
            var clampX = Math.Clamp(Center.X, target.Min.X, target.Max.X);
            var clampY = Math.Clamp(Center.Y, target.Min.Y, target.Max.Y);

            // 거리.
            var dx = clampX - Center.X;
            var dy = clampY - Center.Y;

            // 피타고라스.
            return dx * dx + dy * dy <= RadiusSqr;
        }

        /// <summary>
        /// Hit sphere.
        /// </summary>
        private bool HitSphere(SphereHitbox target)
        {
            // 두 구체 간 거리가 반지름보다 작은 지 검사.
            var distance = Vector2.DistanceSquared(Center, target.Center);

            if (distance < RadiusSqr || distance < RadiusSqr)
            {
                return true;
            }

            return false;
        }
    }
}
