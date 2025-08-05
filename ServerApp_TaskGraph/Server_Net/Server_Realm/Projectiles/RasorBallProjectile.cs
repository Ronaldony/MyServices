using System.Numerics;

namespace Server_Realm.Projectiles
{
    using GameCore;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;
    using Server_Realm.Worlds;

    /// <summary>
    /// RasorBallProjectile.
    /// </summary>
    public class RasorBallProjectile : Projectile
    {
        private readonly SphereHitbox _hitbox;

        public RasorBallProjectile()
            : base(Type_Projectile.RasorBall)
        {
            _hitbox = new SphereHitbox();
        }

        /// <summary>
        /// Setup.
        /// </summary>
        public override void Setup(int projectileId, Unit owner, Vector2 spawnPos, Vector2 angle, int damage, int radius, int velocity, int maxRange, bool isPenetrates, bool isGrenade)
        {
            // Setup hitbox.
            _hitbox.Setup(radius);

            ProjectileId = projectileId;
            Owner = owner;

            SpawnPos = spawnPos;
            Angle = angle;
            Section = SectionId.ToSection(spawnPos);

            Damage = damage;
            Velocity = velocity;
            MaxRange = maxRange;
            MaxRangeSqr = maxRange * maxRange;
            IsPenetrates = isPenetrates;
            IsGrenade = isGrenade;
        }

        /// <summary>
        /// Move for next frame.
        /// </summary>
        public override void MoveNextFrame(double deltaTime)
        {
            Pos = new Vector2(
                x: Pos.X + Angle.X * (float)deltaTime * Velocity,
                y: Pos.Y + Angle.Y * (float)deltaTime * Velocity);

            _hitbox.Center = Pos;

            // Update section.
            Section = SectionId.ToSection(Pos);
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
