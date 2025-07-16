
namespace Server_World.Core.Worlds.Msg
{
    using Server_World.Core.Types;

    /// <summary>
    /// WorldMsg_SpawnPlayer.
    /// </summary>
    public class WorldMsg_SpawnPlayer : WorldMsg
    {
        public WorldMsg_SpawnPlayer()
            : base (Type_WorldMsg.SpawnPlayer)
        {

        }

        /// <summary>
        /// World player.
        /// </summary>
        public World_Player WorldPlayer { get; set; }
    }
}
