namespace Server_Realm.Worlds.Msg
{
    using Framework.Core.Database.Interfaces;
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
            DataObjects = new List<IDataObject>();
        }

        /// <summary>
        /// PlayerEntity.
        /// </summary>
        public PlayerUnit PlayerUnit;

        /// <summary>
        /// DataObjects.
        /// </summary>
        public List<IDataObject> DataObjects;
     }
}
