
namespace Server_World.Core.GameEvents
{
    using Server_World.Core.Projectiles.Players;

    /// <summary>
    /// MoveProjectileDistrict.
    /// 구역 이동 투사체.
    /// </summary>
    public class MoveProjectileDistrict
    {
        public MoveProjectileDistrict()
        {
            Projectiles = new List<Projectile>();
        }

        /// <summary>
        /// Projectiles.
        /// </summary>
        public List<Projectile> Projectiles { get; set; }
    }
}
