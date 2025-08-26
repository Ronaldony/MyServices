using System.Numerics;

namespace Server_Realm.Context.Units
{
    using Server_Realm.Core.Types;
    using Server_Realm.Context.Collisions;
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
        /// 유닛 죽음 여부.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Type_Entity.
        /// </summary>
        Type_Unit UnitType { get; }

        /// <summary>
        /// Collision.
        /// </summary>
        Collision UnitCollision { get; }

        /// <summary>
        /// World section.
        /// </summary>
        WorldSection WorldSection { get; }
    }
}
