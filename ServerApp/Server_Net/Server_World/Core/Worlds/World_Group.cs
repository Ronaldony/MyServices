using System.Collections.Concurrent;

namespace Server_World.Core.Worlds
{
    using GameCore.Datas.Enemy;
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
        public Dictionary<(int sectionX, int sectionY), HashSet<(int sectionX, int sectionY)>> AroundSectionDict { get; set; }

        /// <summary>
        /// 섹션 정보.
        /// key: Section.
        /// value: Section class.
        /// </summary>
        public Dictionary<(int sectionX, int sectionY), Section> SectionDict { get; set; }

        /// <summary>
        /// HuntZone_Player dictionary.
        /// key: Pid.
        /// value: HuntZone_Player.
        /// </summary>
        public Dictionary<string, World_Player> PlayerDict;

        /// <summary>
        /// 섹션 단위 플레이어 정보.
        /// key: Section.
        /// value: Player dictionary (key: pid).
        /// </summary>
        public Dictionary<(int sectionX, int sectionY), ConcurrentDictionary<string, World_Player>> SectionPlayerDict;

        /// <summary>
        /// 적 정보.
        /// key: Section.
        /// value: Wolrd_enemy dictionary (key: EnemeyUId).
        /// </summary>
        public Dictionary<ulong, World_Enemy> EnemyDict;

        /// <summary>
        /// 섹션 단위 적 정보.
        /// key: Section.
        /// value: Wolrd_enemy dictionary (key: EnemeyUId).
        /// </summary>
        public Dictionary<(int sectionX, int sectionY), ConcurrentDictionary<ulong, World_Enemy>> SectionEnemyDict;
    }
}
