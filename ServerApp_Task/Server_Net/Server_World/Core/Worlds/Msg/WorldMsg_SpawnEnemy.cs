namespace Server_World.Core.Worlds.Msg
{
    using GameCore.Datas.Enemy;
    using Server_World.Core.Datas;
    using Server_World.Core.Types;

    /// <summary>
    /// WorldMsg_SpawnEnemy.
    /// </summary>
    public class WorldMsg_SpawnEnemy : WorldMsg
    {
        public WorldMsg_SpawnEnemy()
            : base (Type_WorldMsg.SpawnEnemy)
        {

        }

        /// <summary>
        /// Enemies.
        /// </summary>
        public Dictionary<SectionId, List<DTO_Enemy>> Enemies { get; set; }
    }
}
