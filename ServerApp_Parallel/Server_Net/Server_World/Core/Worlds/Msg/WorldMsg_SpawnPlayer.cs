
namespace Server_World.Core.Worlds.Msg
{
    using Server_World.Core.Entities;
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
        /// PlayerEntity.
        /// </summary>
        public PlayerEntity PlayerEntity { get; set; }
    }
}
