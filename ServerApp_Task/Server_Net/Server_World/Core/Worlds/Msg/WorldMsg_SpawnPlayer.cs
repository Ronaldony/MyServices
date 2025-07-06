namespace Server_World.Core.Worlds.Msg
{
    /// <summary>
    /// WorldMsg_SpawnPlayer.
    /// </summary>
    public class WorldMsg_SpawnPlayer : WorldMsg
    {
        public WorldMsg_SpawnPlayer()
        {

        }

        /// <summary>
        /// World player.
        /// </summary>
        public World_Player WorldPlayer { get; set; }
    }
}
