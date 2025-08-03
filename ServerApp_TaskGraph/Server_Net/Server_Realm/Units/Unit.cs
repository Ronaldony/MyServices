using System.Numerics;

namespace Server_Realm.Entities
{
    using Server_Realm.AI;
    using Server_Realm.Collisions;
    using Server_Realm.Core.Types;
    using Server_Realm.UnitHandler;
    using Server_Realm.Worlds;

    /// <summary>
    /// Unit.
    /// </summary>
    public abstract class Unit : IUnit
    {
        /// <summary>
        /// Unit id.
        /// </summary>
        public long UnitId { get; protected set; }

        /// <summary>
        /// Entity 활성 여부.
        /// </summary>
        public bool IsDead { get; set; }

        /// <summary>
        /// Type_Entity.
        /// </summary>
        public Type_Unit UnitType { get; protected set; }

        /// <summary>
        /// 현재 섹션.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// Collision.
        /// </summary>
        public Collision UnitCollision { get; protected set; }

        /// <summary>
        /// AI head.
        /// </summary>
        public AIBrain AIBrain { get; protected set; }

        /// <summary>
        /// Combat handler.
        /// </summary>
        public UnitCombatHandler CombatHandler { get; protected set; }

        /// <summary>
        /// Movement handler.
        /// </summary>
        public UnitMovementHandler MovementHandler { get; protected set; }

        /// <summary>
        /// Stat handler.
        /// </summary>
        public UnitStatHandler StatHandler { get; protected set; }

        /// <summary>
        /// World section.
        /// </summary>
        public World_Section WorldSection { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
