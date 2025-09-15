using System.Numerics;

namespace Server_Realm.Units.Mob
{
    using Framework.Core.Utilities;
    using GameCore;
    using GameCore.Datas.Mobs;
    using Server_Realm.Context.AI;
    using Server_Realm.Context.UnitHandler;
    using Server_Realm.Context.Units;
    using Server_Realm.Core.Items;
    using Server_Realm.Core.Types;

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
        public void Setup(IServiceProvider serviceProvider, long unitId, DTO_Mob dtoMob, Dictionary<int, List<DropItem>> dropItems, int areaSpawnId)
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
            SpawnPos = dtoMob.SpawnPos;
            AIBrain = new AIBrain(serviceProvider, this);
            AIBrain.ChangeState(Type_AIState.Roam);

            // Items.
            DropItems = dropItems;

            // TB_AreaSpawn.
            AreaSpawnId = areaSpawnId;
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
        /// Drop items.
        /// key: aggro order.
        /// value: DropItems.
        /// </summary>
        public Dictionary<int, List<DropItem>> DropItems { get; private set; }

        /// <summary>
        /// TB_AreaSpawn id.
        /// </summary>
        public int AreaSpawnId { get; private set; }

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
