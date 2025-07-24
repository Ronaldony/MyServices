namespace Server_Realm.GameEvents
{
    using Framework.Core.Session;
    using Server_Realm.Core.Types;

    /// <summary>
    /// MoveDistrictEvent.
    /// 이벤트 처리
    /// 1) 섹션을 이동 플레이어에게 구역의 새로운 시야 정보 전송.
    /// </summary>
    public class MoveSectionEvent : GameEvent
    {
        public MoveSectionEvent()
            : base(Type_GameEvent.MoveSection)
        {
        }

        /// <summary>
        /// Move pids.
        /// </summary>
        public ICollection<string> MovePids { get; set; }
    }
}
