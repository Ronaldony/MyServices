
namespace Server_World.Core.Collisions.Boundings
{
    using Server_World.Core.Types;

    /// <summary>
    /// SquareHitbox.
    /// </summary>
    public class SquareHitbox : Hitbox
    {
        public SquareHitbox() 
            : base (Type_Hitbox.Square)
        {
        }

        public float MinX { get; set; }

        public float MinY { get; set; }

        public float MaxX { get; set; }

        public float MaxY { get; set; }

        /// <summary>
        /// Check hit with target hit box.
        /// </summary>
        public override bool IsHit(Hitbox target)
        {
            bool isHit = false;

            switch (target.GetHitboxType())
            {
                case Type_Hitbox.Square:
                    {
                        var tagetHitbox = target as SquareHitbox;
                        isHit = HitSquare(tagetHitbox);
                    }
                    break;
                case Type_Hitbox.Sphere:
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
            if (this.MinX > target.MaxX || this.MaxX < target.MinX)
            {
                return false;
            }

            // 세로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (this.MinY > target.MaxY && this.MaxY < target.MinY)
            {
                return false;
            }

            return true;
        }
    }
}
