namespace Server_Realm.Worlds
{
    using Server_Realm.Entities;
    using Server_Realm.Units.Enemy;

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
        /// Player unit 리스트.
        /// key: UnitId.
        /// value: PlayerUnit.
        /// </summary>
        public Dictionary<long, PlayerUnit> PlayerUnitDict { get; set; }

        /// <summary>
        /// Mob unit 리스트.
        /// key: UnitId.
        /// value: MobUnit.
        /// </summary>
        public Dictionary<long, MobUnit> MobUnitDict { get; set; }

        /// <summary>
        /// 섹션 단위 정보.
        /// key: section.
        /// value: world section.
        /// </summary>
        public Dictionary<SectionId, World_Section> WorldSectionDict;
    }
}
