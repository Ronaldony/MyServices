using System.Numerics;

namespace Server_Realm.Worlds.Msg
{
    using GameCore;
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_UpdateMovePattern.
    /// </summary>
    public class WorldMsg_UpdateMovePattern : WorldMsg
    {
        public WorldMsg_UpdateMovePattern()
            : base(Type_WorldMsg.UpdateMovePattern)
        {
        }

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
