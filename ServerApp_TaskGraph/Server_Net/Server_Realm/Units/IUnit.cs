using System.Numerics;

namespace Server_Realm.Entities
{
    using Server_Realm.Collisions;
    using Server_Realm.Core.Types;
    using Server_Realm.Worlds;

    /// <summary>
    /// IUnit.
    /// </summary>
    public interface IUnit : IDisposable
    {
        /// <summary>
        /// Unit id.
        /// </summary>
        long UnitId { get; }

        /// <summary>
        /// Entity 활성 여부.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Type_Entity.
        /// </summary>
        Type_Unit UnitType { get; }

        /// <summary>
        /// 현재 위치.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// 현재 섹션.
        /// </summary>
        SectionId Section { get; }

        /// <summary>
        /// Collision.
        /// </summary>
        Collision UnitCollision { get; }
    }
}
