namespace Server_World.Core.Worlds.Msg
{
    using GameCore.Datas.Enemy;
    using Server_World.Core.Datas;

    /// <summary>
    /// WorldMsg_SpawnEnemy.
    /// </summary>
    public class WorldMsg_SpawnEnemy : WorldMsg
    {
        public WorldMsg_SpawnEnemy()
        {

        }

        /// <summary>
        /// Enemies.
        /// </summary>
        public Dictionary<SectionId, List<DTO_Enemy>> Enemies { get; set; }
    }
}
