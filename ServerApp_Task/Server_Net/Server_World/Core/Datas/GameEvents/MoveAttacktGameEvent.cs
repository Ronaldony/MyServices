namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;

    /// <summary>
    /// MoveAttacktGameEvent.
    /// </summary>
    public class MoveAttacktGameEvent : GameEvent
    {
        public MoveAttacktGameEvent()
            : base (Type_GameEvent.Attack)
        {
        }
    }
}
