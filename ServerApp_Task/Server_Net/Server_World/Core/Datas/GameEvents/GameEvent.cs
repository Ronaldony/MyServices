namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;

    /// <summary>
    /// GameEvent.
    /// </summary>
    public abstract class GameEvent
    {
        public GameEvent(Type_GameEvent gameEventType)
        {
            GameEventType = gameEventType;
        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 출처 섹션.
        /// </summary>
        public SectionId FromSection { get; set; }

        /// <summary>
        /// Game event.
        /// </summary>
        public Type_GameEvent GameEventType { get; set;}
    }
}
