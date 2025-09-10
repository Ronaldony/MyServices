namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;

    /// <summary>
    /// WorldMsg_EnterPlayer.
    /// </summary>
    public class WorldMsg_EnterPlayer : WorldMsgBase
    {
        public WorldMsg_EnterPlayer()
            : base(Type_WorldMsg.EnterPlayer)
        {

        }

        /// <summary>
        /// PlayerEntity.
        /// </summary>
        public PlayerUnit PlayerUnit;
    }
}
