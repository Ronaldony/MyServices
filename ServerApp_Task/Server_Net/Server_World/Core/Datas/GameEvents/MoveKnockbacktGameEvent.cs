namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;

    /// <summary>
    /// MoveKnockbacktGameEvent.
    /// </summary>
    public class MoveKnockbacktGameEvent : GameEvent
    {
        public MoveKnockbacktGameEvent()
            : base (Type_GameEvent.KnockBack)
        {
        }
    }
}
