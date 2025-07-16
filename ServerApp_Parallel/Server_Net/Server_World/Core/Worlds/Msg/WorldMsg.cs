using Server_World.Core.Types;

namespace Server_World.Core.Worlds.Msg
{
    /// <summary>
    /// WorldMsg.
    /// </summary>
    public abstract class WorldMsg
    {
        public WorldMsg(Type_WorldMsg msgType)
        {
            MsgType = msgType;
        }

        /// <summary>
        /// Type_WorldMsg.
        /// </summary>
        public Type_WorldMsg MsgType { get; set; }
    }
}
