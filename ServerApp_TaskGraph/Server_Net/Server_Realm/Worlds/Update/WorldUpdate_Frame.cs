using System.Collections.Concurrent;

namespace Server_Realm.Worlds.Update
{
    /// <summary>
    /// WorldUpdate_Frame.
    /// </summary>
    public class WorldUpdate_Frame : IDisposable
    {
        public WorldUpdate_Frame()
        {
            MoveSectionUnits = new ConcurrentDictionary<SectionId, WorldUpdate_SectionUnit>();
            HitUnits = new ConcurrentDictionary<SectionId, WorldUpdate_HitUnit>();
        }

        /// <summary>
        /// 섹션 이동 유닛.
        /// key: 섹션.
        /// value: 이동 유닛.
        /// </summary>s
        public ConcurrentDictionary<SectionId, WorldUpdate_SectionUnit> MoveSectionUnits { get; set; }

        /// <summary>
        /// 피격 유닛.
        /// key: 섹션.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, WorldUpdate_HitUnit> HitUnits { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            MoveSectionUnits.Clear();
            HitUnits.Clear();
        }
    }
}
