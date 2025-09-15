using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;

namespace Server_Realm.Core.Factories
{
    using GameCore;
    using Server_Realm.Context.Units;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// UnitFactory.
    /// </summary>
    public sealed class UnitFactory
    {
        private readonly ILogger<UnitFactory> _logger;

        private readonly IMemoryPoolService _memoryPoolService;
        private readonly CollisionFactory _collisionFactory;

        public UnitFactory(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitFactory>>();

            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _collisionFactory = serviceProvider.GetRequiredService<CollisionFactory>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation($"UnitFactory initialize.");
        }

        /// <summary>
        /// Create unit.
        /// </summary>
        public T CreateUnit<T>(Type_Unit unitType) where T : Unit
        {
            switch (unitType)
            {
                case Type_Unit.Player:
                    var pu = _memoryPoolService.Rent<PlayerUnit>();

                    // Set collision.
                    if (null == pu.UnitCollision) pu.UnitCollision = _collisionFactory.Create(Type_Collision.Player);

                    return pu as T;
                case Type_Unit.Mob:
                    var mu = _memoryPoolService.Rent<MobUnit>();

                    // Set collision.
                    if (null == mu.UnitCollision) mu.UnitCollision = _collisionFactory.Create(Type_Collision.Mob);

                    return mu as T;
            }

            return null;
        }

        /// <summary>
        /// Return unit.
        /// </summary>
        public void Return(Unit unit)
        {
            if (null == unit)
            {
                return;
            }

            switch (unit.UnitType)
            {
                case Type_Unit.Player:
                    _memoryPoolService.Release<PlayerUnit>(unit as PlayerUnit);
                    break;
                case Type_Unit.Mob:
                    _memoryPoolService.Release<MobUnit>(unit as MobUnit);
                    break;
            }
        }
    }
}
