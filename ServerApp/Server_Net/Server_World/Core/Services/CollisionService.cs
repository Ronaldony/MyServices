using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_World.Core.Services
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_World.Core.Collision;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// CollisionService.
    /// </summary>
    public class CollisionService
    {
        private readonly ILogger<CollisionService> _logger;

        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;

        private IEnumerable<TB_Collision> _tbCollisions;
        private IEnumerable<TB_Weapon> _tbWeapons;

        public CollisionService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CollisionService>>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _tbCollisions = _tdbService.SelectAll<TB_Collision>();
            _tbWeapons = _tdbService.SelectAll<TB_Weapon>();

            _logger.LogInformation("CollisionService initialized.");
        }

        /// <summary>
        /// Get collision box.
        /// </summary>
        public CollisionBox Rent(Type_Collision collisionType)
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
        /// Get collision box.
        /// </summary>
        public CollisionBox Rent_Weapon(int weapoinId)
        {
            var tbWeapon = _tbWeapons.FirstOrDefault(d => d.Id == weapoinId);

            if (default == tbWeapon)
            {
                return null;
            }

            var tbCollision = _tbCollisions.FirstOrDefault(d => d.CollisionType == tbWeapon.CollisionType);

            if (default == tbCollision)
            {
                return null;
            }

            // Collision box.
            var collisionBox = TakeCollisionBox(tbWeapon.CollisionType);

            collisionBox.Setup(tbWeapon.CollisionType, tbCollision.Width, tbCollision.Height, tbCollision.OffsetX, tbCollision.OffsetY);

            return collisionBox;
        }

        /// <summary>
        /// Release collision box.
        /// </summary>
        public void Release(CollisionBox collisionBox)
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
        public bool CollidesWith(BoudingBox src, BoudingBox dst)
        {
            // 가로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (src.Min.X > dst.Max.X || src.Max.X < dst.Min.X)
            {
                return false;
            }

            // 세로 충돌.
            // Cond 1. src의 왼쪽 변이 dst의 오른쪽보다 왼쪽인 경우.
            // Cond 2. src의 오른쪽 변이 dst의 왼쪽 변보다 오른쪽인 경우.
            if (src.Min.Y > dst.Max.Y && src.Max.Y < dst.Min.Y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Take collision box.
        /// </summary>
        private CollisionBox TakeCollisionBox(Type_Collision collisionType)
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
