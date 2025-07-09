
namespace Server_World.Core.Worlds
{
    using Server_World.Core.Datas;

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
        /// 주변 섹션 그룹.
        /// key: SectionId.
        /// value: Around section group.
        /// </summary>
        public Dictionary<SectionId, HashSet<SectionGroup>> AroundSectionGroupDict;

        /// <summary>
        /// 섹션 그룹.
        /// key: SectionId.
        /// value: SectionGroup.
        /// </summary>
        public Dictionary<SectionId, SectionGroup> SectionGroupDict;
    }
}
