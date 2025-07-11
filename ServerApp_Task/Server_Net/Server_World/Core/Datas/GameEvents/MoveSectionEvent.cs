
namespace Server_World.Core.Datas.GameEvents
{
    using Framework.Core.Session;
    using Server_World.Core.Types;

    /// <summary>
    /// MoveDistrictEvent.
    /// 이벤트 처리
    /// 1) 섹션을 이동 플레이어에게 구역의 새로운 시야 정보 전송.
    /// </summary>
    public class MoveSectionEvent : GameEvent
    {
        public MoveSectionEvent()
            : base (Type_GameEvent.MoveSection)
        {
        }

        /// <summary>
        /// GameSessions.
        /// </summary>
        public ICollection<GameSession> GameSessions { get; set; }
    }
}
