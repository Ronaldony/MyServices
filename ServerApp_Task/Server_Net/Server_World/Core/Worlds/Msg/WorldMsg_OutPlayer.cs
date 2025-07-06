namespace Server_World.Core.Worlds.Msg
{
    /// <summary>
    /// WorldMsg_OutPlayer.
    /// </summary>
    public class WorldMsg_OutPlayer : WorldMsg
    {
        public WorldMsg_OutPlayer()
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }
    }
}
