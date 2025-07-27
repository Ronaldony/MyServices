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
        public long UnitId { get; init; }

        /// <summary>
        /// Entity 활성 여부.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Type_Entity.
        /// </summary>
        public Type_Unit UnitType { get; init; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// 현재 섹션.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// Collision.
        /// </summary>
        public Collision UnitCollision { get; init; }

        /// <summary>
        /// AI head.
        /// </summary>
        public AIBrain AIBrain { get; init; }

        /// <summary>
        /// Combat handler.
        /// </summary>
        public UnitCombatHandler CombatHandler { get; init; }

        /// <summary>
        /// Movement handler.
        /// </summary>
        public UnitMovementHandler MovementHandler { get; init; }

        /// <summary>
        /// Stat handler.
        /// </summary>
        public UnitStatHandler StatHandler { get; init; }

        /// <summary>
        /// Update.
        /// </summary>
        public abstract void Update(double deltaTime);

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
