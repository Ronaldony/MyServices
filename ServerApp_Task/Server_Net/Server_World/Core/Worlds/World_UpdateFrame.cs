
namespace Server_World.Core.Worlds
{
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds.Section;
    using System.Collections.Concurrent;

    /// <summary>
    /// World_UpdateMove.
    /// </summary>
    public class World_UpdateFrame : IDisposable
    {
        public World_UpdateFrame()
        {
            MoveSectionUnit = new ConcurrentDictionary<SectionId, World_SectionUnit>();
            MoveDistrictUnit = new ConcurrentDictionary<SectionId, World_SectionUnit>();
            AttackSectionUnit = new Dictionary<SectionId, World_SectionUnit>();
        }

        /// <summary>
        /// Move section.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_SectionUnit> MoveSectionUnit { get; set; }

        /// <summary>
        /// Move section.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_SectionUnit> MoveDistrictUnit { get; set; }

        /// <summary>
        /// Move section.
        /// </summary>
        public Dictionary<SectionId, World_SectionUnit> AttackSectionUnit { get; set; }

        public void Dispose()
        {
            MoveSectionUnit.Clear();
            MoveDistrictUnit.Clear();
            AttackSectionUnit.Clear();
        }
    }
}
