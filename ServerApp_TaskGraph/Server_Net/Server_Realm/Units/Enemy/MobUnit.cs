using Server_Realm.Entities;

namespace Server_Realm.Units.Enemy
{
    using GameCore.Datas.Mobs;
    using Server_Realm.AI;
    using Server_Realm.Collisions;
    using Server_Realm.Core.Types;
    using Server_Realm.UnitHandler;
    using Server_Realm.Worlds;

    /// <summary>
    /// EnemyUnit.
    /// </summary>
    public class MobUnit : Unit
    {
        public MobUnit(IServiceProvider serviceProvider, long unitId, DTO_Mob dtoMob, Collision collision)
        {
            // Unit.
            UnitId = unitId;
            IsActive = true;
            UnitType = Type_Unit.Enemy;
            Position = dtoMob.Location.Pos;
            Section = SectionId.WorldPosToSection(Position);
            UnitCollision = collision;
            AIBrain = new AIBrain(this);

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider, this);
            MovementHandler = new UnitMovementHandler(serviceProvider, this, dtoMob.Location, dtoMob.Stat);
            StatHandler = new UnitStatHandler(serviceProvider, this, dtoMob.Stat);
        }

        #region props
        /// <summary>
        /// Mob.
        /// </summary>
        public DTO_Mob Mob { get; }
        #endregion

        /// <summary>
        /// Update.
        /// </summary>
        public override void Update(double deltaTime)
        {
            CombatHandler.Update(deltaTime);
            MovementHandler.Update(deltaTime);
            StatHandler.Update(deltaTime);

            AIBrain.Update(deltaTime);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
