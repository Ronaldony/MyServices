using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Core.Services.Interfaces;

namespace Server_World.Core.Services
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_World.Core.Collision;

    /// <summary>
    /// CollisionManager.
    /// </summary>
    public static class CollisionManager
    {
        private static ITDBService _tdbService;
        private static IMemoryPoolService _memoryPoolService;

        private static IEnumerable<TB_Collision> _tbCollisions;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            _tbCollisions = _tdbService.SelectAll<TB_Collision>();
        }

        /// <summary>
        /// Get collision box.
        /// </summary>
        public static CollisionBox Rent(Type_Collision collisionType)
        {
            var tbCollision = _tbCollisions.FirstOrDefault(d => d.CollisionType == collisionType);

            if (default == tbCollision)
            {
                return null;
            }
            
            // Collision box.
            var collisionBox = TakeCollisionBox(collisionType);

            collisionBox.Setup(collisionType, tbCollision.Width, tbCollision.Height, tbCollision.OffsetX, tbCollision.OffsetY);

            return collisionBox;
        }

        /// <summary>
        /// Release collision box.
        /// </summary>
        public static void Release(CollisionBox collisionBox)
        {
            switch (collisionBox.CollisionType)
            {
                case Type_Collision.Player:
                case Type_Collision.Enemy:
                    _memoryPoolService.Release<UnitCollisionBox>(collisionBox as UnitCollisionBox);
                    break;
                case Type_Collision.Sword:
                case Type_Collision.Arrow:
                case Type_Collision.Rasor:
                    _memoryPoolService.Release<SwordCollisionBox>(collisionBox as SwordCollisionBox);
                    break;
            }
        }

        /// <summary>
        /// Check collision status.
        /// </summary>
        public static bool CollidesWith(BoudingBox src, BoudingBox dst)
        {
            // 가로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (src.MinX > dst.MaxX || src.MaxX < dst.MinX)
            {
                return false;
            }

            // 세로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (src.MinY > dst.MaxY && src.MaxY < dst.MinY)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Take collision box.
        /// </summary>
        private static CollisionBox TakeCollisionBox(Type_Collision collisionType)
        {
            CollisionBox collisionBox;

            switch (collisionType)
            {
                case Type_Collision.Player:
                case Type_Collision.Enemy:
                    collisionBox = _memoryPoolService.Rent<UnitCollisionBox>();
                    break;
                case Type_Collision.Sword:
                case Type_Collision.Arrow:
                case Type_Collision.Rasor:
                    collisionBox = _memoryPoolService.Rent<SwordCollisionBox>();
                    break;
                default:
                    return null;
            }

            return collisionBox;
        }
    }
}
