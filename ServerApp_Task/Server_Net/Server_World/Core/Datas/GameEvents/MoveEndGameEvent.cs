using System.Numerics;

namespace Server_World.Core.Datas.GameEvents
{
    using GameCore;
    using Server_World.Core.Types;

    /// <summary>
    /// MoveEndGameEvent.
    /// </summary>
    public class MoveEndGameEvent : GameEvent
    {
        public MoveEndGameEvent() 
            : base (Type_GameEvent.MoveEnd)
        {
        }

        /// <summary>
        /// Packet time.
        /// </summary>
        public DateTime PacketTime { get; set; }

        /// <summary>
        /// End position.
        /// </summary>
        public Vector2 EndPos { get; set; }

        /// <summary>
        /// Action.
        /// </summary>
        public Type_Action Action { get; set; }
    }
}
