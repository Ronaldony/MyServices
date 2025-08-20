using System.Numerics;

namespace Server_Realm.Projectiles
{
    using GameCore;
    using Server_Realm.Entities;
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
        public override void Setup(int projectileId, Unit owner, Vector2 spawnPos, Vector2 endPos, int damage, TB_Projectile tbProjectile)
        {
            // Setup base.
            base.Setup(projectileId, owner, spawnPos, endPos, damage, tbProjectile);

            // Setup hitbox.
            _hitbox.Setup(tbProjectile.Radius, spawnPos);
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
