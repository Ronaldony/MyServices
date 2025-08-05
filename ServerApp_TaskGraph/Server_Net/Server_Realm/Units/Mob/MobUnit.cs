using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace Server_Realm.Units.Mob
{
    using GameCore.Datas.Mobs;
    using Server_Realm.AI;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Types;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Entities;
    using Server_Realm.UnitHandler;

    /// <summary>
    /// MobUnit.
    /// </summary>
    public partial class MobUnit : Unit
    {
        public MobUnit()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            // Unit.
            UnitType = Type_Unit.Mob;

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider);
            MovementHandler = new UnitMovementHandler(serviceProvider);
            StatHandler = new UnitStatHandler(serviceProvider);
        }

        #region setup
        /// <summary>
        /// Setup.
        /// </summary>
        public void Setup(IServiceProvider serviceProvider, long unitId, DTO_Mob dtoMob)
        {
            // Unit.
            IsDead = false;
            UnitId = unitId;
            UnitType = Type_Unit.Mob;

            // handler.
            CombatHandler.Setup(this);
            MovementHandler.Setup(this, dtoMob.Location, dtoMob.Stat);
            StatHandler.Setup(this, dtoMob.Stat);

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
            // Dispose.
            CombatHandler.Dispose();
        }
    }
}
