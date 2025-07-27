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
            MoveDistrictUnits = new ConcurrentDictionary<SectionId, WorldUpdate_DistrictUnit>();
            HitUnits = new ConcurrentDictionary<SectionId, WorldUpdate_HitUnit>();
            MoveDistrictProjectiles = new ConcurrentDictionary<SectionId, WorldUpdate_DistrictProjectile>();
        }

        /// <summary>
        /// 섹션 이동 유닛.
        /// key: 섹션.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, WorldUpdate_SectionUnit> MoveSectionUnits { get; set; }

        /// <summary>
        /// 구역 이동 유닛.
        /// key: 이동 구역.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, WorldUpdate_DistrictUnit> MoveDistrictUnits { get; set; }

        /// <summary>
        /// 피격 유닛.
        /// key: 섹션.
        /// value: 이동 유닛.
        /// </summary>
        public ConcurrentDictionary<SectionId, WorldUpdate_HitUnit> HitUnits { get; set; }

        /// <summary>
        /// 구역 이동 투사체.
        /// key: 이동 구역.
        /// value: 이동 투사체.
        /// </summary>
        public ConcurrentDictionary<SectionId, WorldUpdate_DistrictProjectile> MoveDistrictProjectiles { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            MoveSectionUnits.Clear();
            MoveDistrictUnits.Clear();
            HitUnits.Clear();
            MoveDistrictProjectiles.Clear();
        }
    }
}
