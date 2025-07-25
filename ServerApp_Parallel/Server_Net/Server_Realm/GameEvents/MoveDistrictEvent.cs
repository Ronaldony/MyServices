namespace Server_Realm.GameEvents
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Units.Enemy;
    using Server_Realm.Worlds;

    /// <summary>
    /// MoveDistrictEvent.
    /// 이벤트 처리.
    /// 1) 구역 이동 플레이어에게 새로운 시야 정보 전송.
    /// </summary>
    public class MoveDistrictEvent : GameEvent
    {
        public MoveDistrictEvent()
            : base(Type_GameEvent.MoveDistrict)
        {
            PlayerEntities = new List<PlayerUnit>();
            EnemyEntities = new List<EnemyUnit>();
        }

        /// <summary>
        /// 출처 구역.
        /// </summary>
        public District FromDistrict { get; set; }

        /// <summary>
        /// World_Player.
        /// </summary>
        public List<PlayerUnit> PlayerEntities { get; set; }

        /// <summary>
        /// World_Enemy.
        /// </summary>
        public List<EnemyUnit> EnemyEntities { get; set; }
    }
}
