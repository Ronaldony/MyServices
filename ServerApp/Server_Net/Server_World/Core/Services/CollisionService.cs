using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_World.Core.Services
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_World.Core.Datas;

    /// <summary>
    /// CollisionService.
    /// </summary>
    public class CollisionService
    {
        private readonly ILogger<CollisionService> _logger;
        private readonly ITDBService _tdbService;

        private IEnumerable<TB_Collision> _tbCollisions;
        private IEnumerable<TB_Weapon> _tbWeapons;

        public CollisionService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CollisionService>>();
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _tbCollisions = _tdbService.SelectAll<TB_Collision>();

            _logger.LogInformation("CollisionService initialized.");
        }

        /// <summary>
        /// Calculation for collision box.
        /// </summary>
        public CollisionBox CalcCollisionBox(int weapon, Vector2 playerPos, Vector2 attackAngle)
        {
            var tbWeapon = _tbWeapons.FirstOrDefault(d => d.Id == weapon);

            if (tbWeapon == default)
            {
                return default;
            }

            var tbCollision = _tbCollisions.FirstOrDefault(d => d.CollisionType == tbWeapon.CollisionType);

            if (default == tbCollision)
            {
                return default;
            }

            var collisionBox = new CollisionBox();

            return collisionBox;
        }

        /// <summary>
        /// Get player collision box.
        /// </summary>
        private CollisionBox GetPlayerCollisionBox(Vector2 playerPos)
        {
            var tbCollision = _tbCollisions.FirstOrDefault(d => d.CollisionType == Type_Collision.Player);

            return new CollisionBox
            {
                //Point1 = new Vector2(playerPos.X + tbCollision.Point1_x, playerPos.Y + tbCollision.Point1_y),
                //Point2 = new Vector2(playerPos.X + tbCollision.Point2_x, playerPos.Y + tbCollision.Point2_y),
                //Point3 = new Vector2(playerPos.X + tbCollision.Point3_x, playerPos.Y + tbCollision.Point3_y),
                //Point4 = new Vector2(playerPos.X + tbCollision.Point4_x, playerPos.Y + tbCollision.Point4_y)
            };
        }

        /// <summary>
        /// Check collision
        /// </summary>
        public bool IsCollided(Vector2 playerPos, CollisionBox collisionBox)
        {
            var playerCollisionBox = GetPlayerCollisionBox(playerPos);



            return false;
        }
    }
}
