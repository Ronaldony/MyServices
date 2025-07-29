using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Numerics;

namespace Server_Realm.Core.Factory
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Entities;
    using Server_Realm.Projectiles;

    /// <summary>
    /// ProjectileFactory.
    /// </summary>
    public class ProjectileFactory
    {
        private readonly ILogger<ProjectileFactory> _logger;

        private readonly ITDBService _tdbService;
        private readonly IMemoryPoolService _memoryPoolService;

        private readonly Dictionary<Type_Projectile, TB_Projectile> _tbProjectileDict;

        public ProjectileFactory(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ProjectileFactory>>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            // TB_Projectile.
            _tbProjectileDict = _tdbService.SelectAll<TB_Projectile>().ToDictionary(tb => tb.ProjectileType, tb => tb);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation("ProjectileFactory initialized");
        }

        /// <summary>
        /// Create projectile.
        /// </summary>
        public Projectile Create(Unit owner, Type_Projectile projectileType, Vector2 spawnPos, Vector2 angle, int damage)
        {
            var projectile = GetProjectile(projectileType);
            if (projectile == null)
            {
                return null;
            }

            var tbProjectile = _tbProjectileDict.GetValueOrDefault(projectileType);

            // Steup projectile.
            projectile.Setup(tbProjectile.Id, owner, spawnPos, angle, damage, tbProjectile.Radius, tbProjectile.Velocity, tbProjectile.MaxRange, tbProjectile.IsPenetrates, tbProjectile.IsGrenade);

            return projectile;
        }

        /// <summary>
        /// Return projectile.
        /// </summary>
        public void Return(Projectile projectile)
        {
            switch (projectile.ProjectilType)
            {
                case Type_Projectile.Arrow:
                    _memoryPoolService.Release(projectile as ArrowProjectile);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get projectile.
        /// </summary>
        private Projectile GetProjectile(Type_Projectile projectileType)
        {
            Projectile projectile;

            switch (projectileType)
            {
                case Type_Projectile.Arrow:
                    projectile = _memoryPoolService.Rent<ArrowProjectile>();
                    break;
                default:
                    return null;
            }

            return projectile;
        }
    }
}
