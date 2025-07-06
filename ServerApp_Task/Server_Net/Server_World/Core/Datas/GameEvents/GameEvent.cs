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
        /// Game event.
        /// </summary>
        public Type_GameEvent GameEventType { get; set;}
    }
}
