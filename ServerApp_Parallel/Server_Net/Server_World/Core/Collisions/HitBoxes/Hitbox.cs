
namespace Server_World.Core.Collisions.Boundings
{
    using Server_World.Core.Types;

    /// <summary>
    /// Hitbox.
    /// </summary>
    public abstract class Hitbox
    {
        /// <summary>
        /// Type_Hitbox.
        /// </summary>
        public Type_Hitbox GetHitboxType() => _hitboxType;
        private readonly Type_Hitbox _hitboxType;

        public Hitbox(Type_Hitbox hitboxType)
        {
            _hitboxType = hitboxType;
        }

        /// <summary>
        /// Check hit with target hit box.
        /// </summary>
        public abstract bool IsHit(Hitbox target);
    }
}
