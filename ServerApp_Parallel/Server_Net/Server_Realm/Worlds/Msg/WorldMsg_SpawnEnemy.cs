namespace Server_Realm.Worlds.Msg
{
    using GameCore.Datas.Enemy;
    using Server_Realm.Core.Types;
    using Server_Realm.Worlds;

    /// <summary>
    /// WorldMsg_SpawnEnemy.
    /// </summary>
    public class WorldMsg_SpawnEnemy : WorldMsg
    {
        public WorldMsg_SpawnEnemy()
            : base(Type_WorldMsg.SpawnEnemy)
        {

        }

        /// <summary>
        /// Enemies.
        /// </summary>
        public Dictionary<SectionId, List<DTO_Enemy>> Enemies { get; set; }
    }
}
