using Server_Realm.Core.Types;

namespace Server_Realm.Worlds.Msg
{
    /// <summary>
    /// WorldMsg.
    /// </summary>
    public abstract class WorldMsgBase
    {
        public WorldMsgBase(Type_WorldMsg msgType)
        {
            MsgType = msgType;
        }

        /// <summary>
        /// Type_WorldMsg.
        /// </summary>
        public Type_WorldMsg MsgType;
    }
}
