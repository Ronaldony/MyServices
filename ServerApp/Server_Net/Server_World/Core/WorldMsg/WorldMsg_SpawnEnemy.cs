namespace Server_World.Core.WorldMsg
{
    using GameCore.Datas.Enemy;

    /// <summary>
    /// WorldMsg_SpawnEnemy.
    /// </summary>
    public class WorldMsg_SpawnEnemy
    {
        /// <summary>
        /// Enemies.
        /// </summary>
        public Dictionary<(int sectionX, int sectionY), List<DTO_Enemy>> Enemies { get; set; }
    }
}
