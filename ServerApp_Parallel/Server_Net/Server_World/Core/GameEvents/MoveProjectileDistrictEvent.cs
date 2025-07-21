
namespace Server_World.Core.GameEvents
{
    using Server_World.Core.Projectiles.Players;
    using Server_World.Core.Types;

    /// <summary>
    /// MoveProjectileDistrict.
    /// 구역 이동 투사체.
    /// </summary>
    public class MoveProjectileDistrictEvent : GameEvent
    {
        public MoveProjectileDistrictEvent()
            : base(Type_GameEvent.MoveDistrictProjectile)
        {
            Projectiles = new List<Projectile>();
        }

        /// <summary>
        /// Projectiles.
        /// </summary>
        public List<Projectile> Projectiles { get; set; }
    }
}
