
namespace Server_World.Core.Worlds
{
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds.Section;

    /// <summary>
    /// World_UpdateMove.
    /// </summary>
    public class World_UpdateFrame
    {
        public World_UpdateFrame()
        {
            MoveSectionUnit = new Dictionary<SectionId, World_SectionUnit>();
            AttackSectionUnit = new Dictionary<SectionId, World_SectionUnit>();
        }

        /// <summary>
        /// Move section.
        /// </summary>
        public Dictionary<SectionId, World_SectionUnit> MoveSectionUnit { get; set; }

        /// <summary>
        /// Move section.
        /// </summary>
        public Dictionary<SectionId, World_SectionUnit> AttackSectionUnit { get; set; }
    }
}
