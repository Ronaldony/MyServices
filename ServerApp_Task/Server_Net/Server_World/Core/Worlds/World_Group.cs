using System.Collections.Concurrent;

namespace Server_World.Core.Worlds
{
    using Server_World.Core.Datas;

    /// <summary>
    /// World_Group.
    /// </summary>
    public class World_Group
    {
        /// <summary>
        /// 근처 섹션 정보.
        /// key: Section.
        /// value: Around sections.
        /// </summary>
        public Dictionary<SectionId, HashSet<SectionId>> AroundSectionDict { get; set; }

        /// <summary>
        /// 관리 섹션.
        /// </summary>
        public HashSet<SectionId> ManagedSections { get; set; }

        /// <summary>
        /// HuntZone_Player dictionary.
        /// key: Pid.
        /// value: World_Player.
        /// </summary>
        public Dictionary<string, World_Player> PlayerDict { get; set; }

        /// <summary>
        /// 섹션 단위 플레이어 정보.
        /// key: Section.
        /// value: Player dictionary (key: pid).
        /// </summary>
        public Dictionary<SectionId, ConcurrentDictionary<string, World_Player>> SectionPlayerDict { get; set; }

        /// <summary>
        /// 적 정보.
        /// key: Section.
        /// value: Wolrd_enemy dictionary (key: EnemeyUId).
        /// </summary>
        public Dictionary<ulong, World_Enemy> EnemyDict { get; set; }

        /// <summary>
        /// 섹션 단위 적 정보.
        /// key: Section.
        /// value: Wolrd_enemy dictionary (key: EnemeyUId).
        /// </summary>
        public Dictionary<SectionId, ConcurrentDictionary<ulong, World_Enemy>> SectionEnemyDict { get; set; }
    }
}
