namespace Server_World.Core.Worlds.Section
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;
    using System.Collections.Concurrent;

    /// <summary>
    /// World_SectionUnit.
    /// </summary>
    public class World_SectionUnit : IDisposable
    {
        public World_SectionUnit()
        {
            PlayInfos = new ConcurrentBag<DTO_PlayInfo>();
            Enemies = new ConcurrentBag<DTO_Enemy>();
        }

        /// <summary>
        /// Player.
        /// </summary>
        public ConcurrentBag<DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// Enemy. 
        /// </summary>
        public ConcurrentBag<DTO_Enemy> Enemies { get; set; }

        public void Dispose()
        {
            PlayInfos.Clear();
            Enemies.Clear();
        }
    }
}
