namespace Server_Realm.GameEvents
{
    using Server_Realm.Core.Types;
    using Server_Realm.Projectiles.Players;

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
