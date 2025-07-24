using System.Numerics;

namespace Server_Realm.Entities
{
    using Server_Realm.Collisions;
    using Server_Realm.Core.Types;
    using Server_Realm.Worlds;

    /// <summary>
    /// Unit.
    /// </summary>
    public abstract class Unit : IUnit
    {
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
        public Collision Collision { get; set; }

        /// <summary>
        /// Update.
        /// </summary>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Dispose.
        /// </summary>
        public abstract void Dispose();
    }
}
