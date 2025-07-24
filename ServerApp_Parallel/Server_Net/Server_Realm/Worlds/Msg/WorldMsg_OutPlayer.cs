namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_OutPlayer.
    /// </summary>
    public class WorldMsg_OutPlayer : WorldMsg
    {
        public WorldMsg_OutPlayer()
            : base(Type_WorldMsg.OutPlayer)
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }
    }
}
