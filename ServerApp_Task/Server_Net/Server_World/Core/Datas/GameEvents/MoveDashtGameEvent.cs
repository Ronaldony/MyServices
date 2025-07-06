namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;

    /// <summary>
    /// MoveDashtGameEvent.
    /// </summary>
    public class MoveDashtGameEvent : GameEvent
    {
        public MoveDashtGameEvent()
            : base (Type_GameEvent.Dash)
        {
        }
    }
}
