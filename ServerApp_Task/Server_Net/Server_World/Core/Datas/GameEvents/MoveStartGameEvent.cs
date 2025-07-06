namespace Server_World.Core.Datas.GameEvents
{
    using GameCore;
    using Server_World.Core.Types;

    /// <summary>
    /// MoveStartGameEvent.
    /// </summary>
    public class MoveStartGameEvent : GameEvent
    {
        public MoveStartGameEvent()
            : base (Type_GameEvent.MoveStart)
        {
        }

        /// <summary>
        /// Update time.
        /// </summary>
        public DateTime PacketTime { get; set; }

        /// <summary>
        /// Move direction.
        /// </summary>
        public Type_MoveDirection MoveDirection { get; set; }

        /// <summary>
        /// Action.
        /// </summary>
        public Type_Action Action { get; set; }
    }
}
