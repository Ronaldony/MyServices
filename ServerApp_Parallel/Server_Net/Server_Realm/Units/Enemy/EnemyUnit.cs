using Server_Realm.Entities;

namespace Server_Realm.Units.Enemy
{
    using GameCore.Datas.Enemy;
    using Server_Realm.AI;
    using Server_Realm.Collisions;
    using Server_Realm.Core.Types;
    using Server_Realm.UnitHandler;
    using Server_Realm.Worlds;
    using Server_Realm.Zones;

    /// <summary>
    /// EnemyUnit.
    /// </summary>
    public class EnemyUnit : Unit
    {
        public EnemyUnit(IServiceProvider serviceProvider, IZoneRouter zoneRouter, DTO_Enemy dtoEnemy, Collision collision)
        {
            // Unit.
            IsActive = true;
            UnitType = Type_Unit.Enemy;
            Position = dtoEnemy.Location.Pos;
            Section = SectionId.GetSection(Position);
            UnitCollision = collision;
            AIBrain = new AIBrain(this);

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider, this, dtoEnemy.Location, dtoEnemy.Stat);
            MovementHandler = new UnitMovementHandler(serviceProvider, this, dtoEnemy.Location, dtoEnemy.Stat);
            StatHandler = new UnitStatHandler(serviceProvider, this, dtoEnemy.Stat);

            // Enemy.
            EnemyUid = dtoEnemy.EnemeyUid;
        }

        /// <summary>
        /// DTO_Enemy.
        /// </summary>
        public DTO_Enemy Enemy { get; }

        /// <summary>
        /// Enemy uid.
        /// </summary>
        public ulong EnemyUid { get; set; }

        /// <summary>
        /// Update.
        /// </summary>
        public override void Update(float deltaTime)
        {
            CombatHandler.Update(deltaTime);
            MovementHandler.Update(deltaTime);
            StatHandler.Update(deltaTime);
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
