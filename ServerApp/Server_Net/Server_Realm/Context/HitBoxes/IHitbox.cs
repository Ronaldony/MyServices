
namespace Server_Realm.Context.HitBoxes
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// Hitbox.
    /// </summary>
    public interface IHitbox
    {
        /// <summary>
        /// Type_Hitbox.
        /// </summary>
        Type_Hitbox HitboxType { get; init; }

        /// <summary>
        /// Check hit with target hit box.
        /// </summary>
        bool IsHit(IHitbox target);
    }
}
