using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;

namespace Server_Realm.Core.Factory
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Collisions;

    /// <summary>
    /// CollisionFactory.
    /// </summary>
    public class CollisionFactory
    {
        private readonly ILogger<CollisionFactory> _logger;

        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;

        private readonly Dictionary<Type_Collision, TB_Collision> _tbCollisionDict;

        public CollisionFactory(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CollisionFactory>>();

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
            _logger.LogInformation("CollisionFactory initialized");
        }

        /// <summary>
        /// Create collision.
        /// </summary>
        public Collision Create(Type_Collision collisionType)
        {
            // Collision.
            var collision = GetCollision(collisionType);

            var tbCollision = _tbCollisionDict.GetValueOrDefault(collisionType);
            collision.Setup(collisionType, tbCollision.Width, tbCollision.Height, tbCollision.OffsetX, tbCollision.OffsetY);

            return collision;
        }

        /// <summary>
        /// Return collision box.
        /// </summary>
        public void Return(Collision collisionBox)
        {
            switch (collisionBox.CollisionType)
            {
                case Type_Collision.Player:
                case Type_Collision.Enemy:
                    _memoryPoolService.Release(collisionBox as UnitCollision);
                    break;
                case Type_Collision.Sword:
                    _memoryPoolService.Release(collisionBox as SwordCollision);
                    break;
            }
        }

        /// <summary>
        /// Get collision.
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
