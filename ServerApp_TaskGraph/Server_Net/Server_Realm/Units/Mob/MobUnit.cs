using System.Numerics;

namespace Server_Realm.Units.Mob
{
    using GameCore.Datas.Mobs;
    using Microsoft.Extensions.DependencyInjection;
    using Server_Realm.AI;
    using Server_Realm.Collisions;
    using Server_Realm.Core.Factory;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.UnitHandler;

    /// <summary>
    /// MobUnit.
    /// </summary>
    public partial class MobUnit : Unit
    {
        #region constructor
        public MobUnit(IServiceProvider serviceProvider, long unitId, DTO_Mob dtoMob, Collision collision)
        {
            // Unit.
            UnitId = unitId;
            UnitType = Type_Unit.Mob;
            UnitCollision = collision;

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider, this);
            MovementHandler = new UnitMovementHandler(serviceProvider, this, dtoMob.Location, dtoMob.Stat);
            StatHandler = new UnitStatHandler(serviceProvider, this, dtoMob.Stat);

            // Factories.
            CollisionFactory = serviceProvider.GetRequiredService<CollisionFactory>();

            // MobUnit.
            Mob = dtoMob;
            SpawnPos = dtoMob.Location.Pos;
            AIBrain = new AIBrain(serviceProvider, this);
        }
        #endregion

        /// <summary>
        /// Spawn position.
        /// </summary>
        public Vector2 SpawnPos { get; private set; }
        /// <summary>
        /// Mob.
        /// </summary>
        public DTO_Mob Mob { get; private set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            // Release memory.
            CollisionFactory.Return(UnitCollision);

            // Dispose.
            CombatHandler.Dispose();
        }
    }
}
