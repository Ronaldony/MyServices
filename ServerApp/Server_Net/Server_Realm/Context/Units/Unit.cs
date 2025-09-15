namespace Server_Realm.Context.Units
{
    using Server_Realm.Context.AI;
    using Server_Realm.Context.Collisions;
    using Server_Realm.Context.UnitHandler;
    using Server_Realm.Core.Types;
    using Server_Realm.Worlds.Sections;
    using Server_Realm.Zones;

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
        /// Collision.
        /// </summary>
        public Collision UnitCollision { get; set; }

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
        public WorldSection WorldSection { get; set; }

        /// <summary>
        /// Zone processor.
        /// </summary>
        public IZoneProcessor ZoneProcessor { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
