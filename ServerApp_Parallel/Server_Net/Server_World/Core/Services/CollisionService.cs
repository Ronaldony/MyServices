using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;

namespace Server_World.Core.Services
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_World.Core.Collisions;

    /// <summary>
    /// CollisionService.
    /// </summary>
    public class CollisionService
    {
        private readonly ILogger<CollisionService> _logger;

        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;

        private readonly Dictionary<Type_Collision, TB_Collision> _tbCollisionDict;

        public CollisionService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CollisionService>>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            // TB_Collision.
            var tbCollisions = _tdbService.SelectAll<TB_Collision>();
            _tbCollisionDict = new Dictionary<Type_Collision, TB_Collision>();

            foreach (var tbCollision in tbCollisions)
            {
                _tbCollisionDict.Add(tbCollision.CollisionType, tbCollision);
            }
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation("CollisionService initialized");
        }

        /// <summary>
        /// Get collision box.
        /// </summary>
        public Collision Rent(Type_Collision collisionType)
        {
            // Collision.
            var collision = GetCollision(collisionType);

            var tbCollision = _tbCollisionDict.GetValueOrDefault(collisionType);
            collision.Setup(collisionType, tbCollision.Width, tbCollision.Height, tbCollision.OffsetX, tbCollision.OffsetY);

            return collision;
        }

        /// <summary>
        /// Release collision box.
        /// </summary>
        public void Release(Collision collisionBox)
        {
            switch (collisionBox.CollisionType)
            {
                case Type_Collision.Player:
                case Type_Collision.Enemy:
                    _memoryPoolService.Release<UnitCollision>(collisionBox as UnitCollision);
                    break;
                case Type_Collision.Sword:
                    _memoryPoolService.Release<SwordCollision>(collisionBox as SwordCollision);
                    break;
            }
        }

        /// <summary>
        /// Take collision box.
        /// </summary>
        private Collision GetCollision(Type_Collision collisionType)
        {
            Collision collisionBox;

            switch (collisionType)
            {
                case Type_Collision.Player:
                case Type_Collision.Enemy:
                    collisionBox = _memoryPoolService.Rent<UnitCollision>();
                    break;
                case Type_Collision.Sword:
                    collisionBox = _memoryPoolService.Rent<SwordCollision>();
                    break;
                default:
                    return null;
            }

            return collisionBox;
        }
    }
}
