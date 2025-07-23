
using System.Collections.Concurrent;

namespace Server_World.Core.Worlds.Section
{
    using Framework.Core.Session;
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// World_SectionUnit.
    /// </summary>
    public class World_SectionUnit : IDisposable
    {
        public World_SectionUnit()
        {
            PlayInfoDict = new ConcurrentDictionary<string, DTO_PlayInfo>();
            Enemies = new ConcurrentBag<DTO_Enemy>();
        }

        /// <summary>
        /// Player.
        /// </summary>
        public ConcurrentDictionary<string, DTO_PlayInfo> PlayInfoDict { get; set; }

        /// <summary>
        /// Enemy. 
        /// </summary>
        public ConcurrentBag<DTO_Enemy> Enemies { get; set; }

        public void Dispose()
        {
            PlayInfoDict.Clear();
            Enemies.Clear();
        }
    }
}
