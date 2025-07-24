namespace Server_Realm.Worlds
{
    /// <summary>
    /// World_SectionGroup.
    /// </summary>
    public class World_SectionGroup
    {
        /// <summary>
        /// 주변 섹션.
        /// key: SectionId.
        /// value: Around sections.
        /// </summary>
        public Dictionary<SectionId, HashSet<SectionId>> AroundSectionDict;

        /// <summary>
        /// 주변 구역.
        /// key: SectionId.
        /// value: Around district.
        /// </summary>
        public Dictionary<SectionId, HashSet<District>> AroundDistrictDict;

        /// <summary>
        /// 구역.
        /// key: SectionId.
        /// value: District.
        /// </summary>
        public Dictionary<SectionId, District> DistrictDict;
    }
}
