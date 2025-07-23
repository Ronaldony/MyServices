namespace Server_World.Core.GameEvents
{
    using Server_World.Core.Entities;
    using Server_World.Core.Types;
    using Server_World.Core.Worlds;

    /// <summary>
    /// MoveDistrictEvent.
    /// 이벤트 처리.
    /// 1) 구역 이동 플레이어에게 새로운 시야 정보 전송.
    /// </summary>
    public class MoveDistrictEvent : GameEvent
    {
        public MoveDistrictEvent()
            : base (Type_GameEvent.MoveDistrict)
        {
            PlayerEntities = new List<PlayerEntity>();
            EnemyEntities = new List<EnemyEntity>();
        }

        /// <summary>
        /// 출처 구역.
        /// </summary>
        public District FromDistrict { get; set; }

        /// <summary>
        /// World_Player.
        /// </summary>
        public List<PlayerEntity> PlayerEntities { get; set; }

        /// <summary>
        /// World_Enemy.
        /// </summary>
        public List<EnemyEntity> EnemyEntities { get; set; }
    }
}
