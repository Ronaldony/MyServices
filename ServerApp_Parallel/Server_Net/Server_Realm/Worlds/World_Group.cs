using Server_Realm.Entities;
using System.Collections.Concurrent;

namespace Server_Realm.Worlds
{
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
        /// HuntZone_Player dictionary.
        /// key: Pid.
        /// value: PlayerEntity.
        /// </summary>
        public Dictionary<string, PlayerUnit> PlayerDict { get; set; }

        /// <summary>
        /// 섹션 단위 플레이어 정보.
        /// key: Section.
        /// value: Player dictionary (key: pid).
        /// </summary>
        public Dictionary<SectionId, ConcurrentDictionary<string, PlayerUnit>> SectionPlayerDict { get; set; }

        /// <summary>
        /// 섹션 단위 적 정보.
        /// key: Section.
        /// value: Wolrd_enemy dictionary (key: EnemeyUId).
        /// </summary>
        public Dictionary<SectionId, ConcurrentDictionary<ulong, EnemyUnit>> SectionEnemyDict { get; set; }
    }
}
