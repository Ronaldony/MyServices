
namespace Server_World.Core.HitBoxes
{
    using Server_World.Core.Types;

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
