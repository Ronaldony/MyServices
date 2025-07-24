using System.Numerics;

namespace Server_Realm.Projectiles.Players
{
    using GameCore;
    using Server_Realm.HitBoxes;
    using Server_Realm.Worlds;

    /// <summary>
    /// ArrowProjectile.
    /// </summary>
    public class ArrowProjectile : Projectile
    {
        private readonly SphereHitbox _hitbox;

        public ArrowProjectile()
            : base(Type_Projectile.Arrow)
        {
            _hitbox = new SphereHitbox();
        }

        /// <summary>
        /// Setup.
        /// </summary>
        public override void Setup(int projectileId, string ownerPid, Vector2 spawnPos, Vector2 angle, int damage, int radius, int velocity, int maxRange, bool isPenetrates)
        {
            // Setup hitbox.
            _hitbox.Setup(radius);

            ProjectileId = projectileId;
            OwnerPid = ownerPid;

            SpawnPos = spawnPos;
            Angle = angle;
            Section = SectionId.GetSection(spawnPos);

            Damage = damage;
            Velocity = velocity;
            MaxRange = maxRange;
            MaxRangeSqr = maxRange * maxRange;
            IsPenetrates = isPenetrates;

            IsActive = true;
        }

        /// <summary>
        /// Move for next frame.
        /// </summary>
        public override void MoveNextFrame(float deltaTime)
        {
            Pos = new Vector2(
                x: Pos.X + Angle.X * deltaTime * Velocity,
                y: Pos.Y + Angle.Y * deltaTime * Velocity);

            _hitbox.Center = Pos;

            // Update section.
            Section = SectionId.GetSection(Pos);
        }

        /// <summary>
        /// Check move to max range or not.
        /// </summary>
        public override bool IsMaxRange()
        {
            if (MaxRangeSqr <= Vector2.DistanceSquared(Pos, SpawnPos))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get hitbox.
        /// </summary>
        public override IHitbox GetHitbox()
        {
            return _hitbox;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
