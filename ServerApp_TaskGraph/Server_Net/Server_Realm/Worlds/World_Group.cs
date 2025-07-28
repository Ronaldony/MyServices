namespace Server_Realm.Worlds
{
    using Server_Realm.Entities;

    /// <summary>
    /// World_Group.
    /// </summary>
    public class World_Group
    {
        /// <summary>
        /// Around section dictionary.
        /// key: source section.
        /// value: around sections.
        /// </summary>
        public Dictionary<SectionId, HashSet<SectionId>> AroundSectionDict;

        /// <summary>
        /// Unit 리스트.
        /// key: UnitId.
        /// value: Unit.
        /// </summary>
        public Dictionary<long, Unit> UnitDict;

        /// <summary>
        /// 섹션 단위 정보.
        /// key: section.
        /// value: world section.
        /// </summary>
        public Dictionary<SectionId, World_Section> WorldSectionDict;
    }
}
