namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_LeavePlayer.
    /// </summary>
    public class WorldMsg_LeavePlayer : WorldMsg
    {
        public WorldMsg_LeavePlayer()
            : base(Type_WorldMsg.LeavePlayer)
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }
    }
}
