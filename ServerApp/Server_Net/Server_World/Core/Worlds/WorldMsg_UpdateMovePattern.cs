
using System.Numerics;

namespace Server_World.Core.Worlds
{
    using GameCore;

    /// <summary>
    /// WorldMsg_UpdateMovePattern.
    /// </summary>
    public class WorldMsg_UpdateMovePattern
    {
        /// <summary>
        /// MovePattern.
        /// </summary>
        public Type_MovePattern MovePattern { get; set; }

        /// <summary>
        /// Move position.
        /// </summary>
        public Vector2 MovePos { get; set; }
    }
}
