namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;

    /// <summary>
    /// WorldMsg_SpawnPlayer.
    /// </summary>
    public class WorldMsg_SpawnPlayer : WorldMsg
    {
        public WorldMsg_SpawnPlayer()
            : base(Type_WorldMsg.SpawnPlayer)
        {

        }

        /// <summary>
        /// PlayerEntity.
        /// </summary>
        public PlayerUnit PlayerUnit { get; set; }
    }
}
