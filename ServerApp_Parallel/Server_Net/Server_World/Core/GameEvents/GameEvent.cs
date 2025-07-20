namespace Server_World.Core.GameEvents
{
    using Server_World.Core.Types;
    using Server_World.Core.Worlds;

    /// <summary>
    /// GameEvent.
    /// </summary>
    public abstract class GameEvent
    {
        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 전파 섹션.
        /// </summary>
        public SectionId RouteSection { get; set; }

        /// <summary>
        /// Game event.
        /// </summary>
        private Type_GameEvent _gameEventType;
        public Type_GameEvent GetGameEventType() => _gameEventType;

        public GameEvent(Type_GameEvent gameEventType)
        {
            _gameEventType = gameEventType;
        }
    }
}
