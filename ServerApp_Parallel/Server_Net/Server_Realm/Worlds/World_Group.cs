
using System.Collections.Concurrent;

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
        /// 관리 섹션.
        /// </summary>
        public HashSet<SectionId> ManagedSections { get; set; }

        /// <summary>
        /// 플레이어 리스트.
        /// key: Pid.
        /// value: PlayerUnit.
        /// </summary>
        public Dictionary<string, PlayerUnit> PlayerDict { get; set; }

        /// <summary>
        /// 섹션 단위 플레이어 정보.
        /// key: Section.
        /// value: PlayerUnit dictionary (key: pid).
        /// </summary>
        public Dictionary<SectionId, ConcurrentDictionary<string, PlayerUnit>> SectionPlayerDict { get; set; }

        /// <summary>
        /// Enemy 리스트.
        /// key: UnitId.
        /// value: EnemyUnit.
        /// </summary>
        public Dictionary<long, MobUnit> MobDict { get; set; }

        /// <summary>
        /// 섹션 단위 적 정보.
        /// key: Section.
        /// value: EnemyUnit dictionary (key: UnitId).
        /// </summary>
        public Dictionary<SectionId, ConcurrentDictionary<long, MobUnit>> SectionMobDict { get; set; }
    }
}
