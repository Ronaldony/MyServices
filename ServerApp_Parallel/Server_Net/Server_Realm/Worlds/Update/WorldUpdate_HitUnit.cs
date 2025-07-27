using GameCore.Datas.Plays;

namespace Server_Realm.Worlds.Update
{
    /// <summary>
    /// WorldUpdate_HitUnit.
    /// </summary>
    public class WorldUpdate_HitUnit : IDisposable
    {
        public WorldUpdate_HitUnit()
        {
            StatDict = new Dictionary<long, DTO_Stat>();
        }

        /// <summary>
        /// 피격 유닛 스텟.
        /// key: unit id.
        /// value: unit stat.
        /// </summary>
        public Dictionary<long, DTO_Stat> StatDict { get; set; }

        public void Dispose()
        {
            StatDict.Clear();
        }
    }
}
