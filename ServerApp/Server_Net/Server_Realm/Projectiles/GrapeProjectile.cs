using System.Numerics;

namespace Server_Realm.Projectiles
{
    using GameCore;
    using Server_Realm.Entities;
    using Server_Realm.HitBoxes;
    using Server_Realm.Worlds;

    /// <summary>
    /// GrenadeProjectile.
    /// </summary>
    public class GrapeProjectile : Projectile
    {
        private readonly SphereHitbox _hitbox;

        private float _timePassed;
        private float _duration;

        public GrapeProjectile()
            : base(Type_Projectile.Grape)
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

            // Setup times.
            _duration = tbProjectile.Duration;
            _timePassed = 0;
        }

        /// <summary>
        /// Move for next frame.
        /// </summary>
        public override void MoveNextFrame(double deltaTime)
        {
            // Delta time.
            _timePassed += (float)deltaTime;

            Pos = Vector2.Lerp(SpawnPos, EndPos, _timePassed / _duration);

            _hitbox.Center = Pos;

            // Update section.
            Section = SectionId.ToSection(Pos);
        }

        /// <summary>
        /// Check move to max range or not.
        /// </summary>
        public override bool IsMaxRange()
        {
            if (GameData.PROJECTILE_ARRIVAL_RANGE_SQUARE >= Vector2.DistanceSquared(Pos, EndPos))
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
