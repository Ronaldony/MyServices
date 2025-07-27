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
        /// Around section dictionary/
        /// key: source section.
        /// value: around sections.
        /// </summary>
        public Dictionary<SectionId, HashSet<SectionId>> AroundSectionDict;

        /// <summary>
        /// 플레이어 리스트.
        /// key: Pid.
        /// value: PlayerUnit.
        /// </summary>
        public Dictionary<string, PlayerUnit> PlayerDict;

        /// <summary>
        /// Enemy 리스트.
        /// key: UnitId.
        /// value: EnemyUnit.
        /// </summary>
        public Dictionary<long, MobUnit> MobDict;

        /// <summary>
        /// 섹션 단위 정보.
        /// key: section.
        /// value: world section.
        /// </summary>
        public Dictionary<SectionId, World_Section> WorldSectionDict;
    }
}
