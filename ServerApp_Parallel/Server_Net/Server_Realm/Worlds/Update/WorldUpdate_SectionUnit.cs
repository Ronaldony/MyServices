using System.Collections.Concurrent;

namespace Server_Realm.Worlds.Update
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// WorldUpdate_SectionUnit.
    /// </summary>
    public class WorldUpdate_SectionUnit : IDisposable
    {
        public WorldUpdate_SectionUnit()
        {
            PlayInfoDict = new ConcurrentDictionary<long, DTO_PlayInfo>();
            MobDict = new ConcurrentDictionary<long, DTO_Enemy>();
        }

        /// <summary>
        /// Player.
        /// </summary>
        public ConcurrentDictionary<long, DTO_PlayInfo> PlayInfoDict { get; set; }

        /// <summary>
        /// Enemy. 
        /// </summary>
        public ConcurrentDictionary<long, DTO_Enemy> MobDict { get; set; }

        public void Dispose()
        {
            PlayInfoDict.Clear();
            MobDict.Clear();
        }
    }
}
