using System.Collections.Concurrent;

namespace Server_Realm.Worlds
{
    using Server_Realm.Worlds.Section;

    /// <summary>
    /// World_UpdateMove.
    /// </summary>
    public class World_UpdateFrame : IDisposable
    {
        public World_UpdateFrame()
        {
            MoveSectionUnit = new ConcurrentDictionary<SectionId, World_SectionUnit>();
            MoveDistrictUnit = new ConcurrentDictionary<SectionId, World_DistrictUnit>();
            HitSectionUnit = new ConcurrentDictionary<SectionId, World_SectionUnit>();
            MoveDistrictProjectile = new ConcurrentDictionary<SectionId, World_DistrictProjectile>();
        }

        /// <summary>
        /// 섹션 이동 유닛.
        /// key: 섹션.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_SectionUnit> MoveSectionUnit { get; set; }

        /// <summary>
        /// 구역 이동 유닛.
        /// key: 이동 구역.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_DistrictUnit> MoveDistrictUnit { get; set; }

        /// <summary>
        /// 피격 유닛.
        /// key: 섹션.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_SectionUnit> HitSectionUnit { get; set; }

        /// <summary>
        /// 구역 이동 투사체.
        /// key: 이동 구역.
        /// value: 이동 투사체.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_DistrictProjectile> MoveDistrictProjectile { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            MoveSectionUnit.Clear();
            MoveDistrictUnit.Clear();
            HitSectionUnit.Clear();
            MoveDistrictProjectile.Clear();
        }
    }
}
