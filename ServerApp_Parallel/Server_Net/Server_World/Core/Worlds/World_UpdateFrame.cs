
using System.Collections.Concurrent;

namespace Server_World.Core.Worlds
{
    using Server_World.Core.Datas;
    using Server_World.Core.Worlds.Section;

    /// <summary>
    /// World_UpdateMove.
    /// </summary>
    public class World_UpdateFrame : IDisposable
    {
        public World_UpdateFrame()
        {
            MoveSectionUnit = new ConcurrentDictionary<SectionId, World_SectionUnit>();
            MoveDistrictUnit = new ConcurrentDictionary<SectionId, World_DistrictUnit>();
            AttackSectionUnit = new ConcurrentDictionary<SectionId, World_SectionUnit>();
        }

        /// <summary>
        /// Move section.
        /// key: moved section.
        /// value: SectionUnit.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_SectionUnit> MoveSectionUnit { get; set; }

        /// <summary>
        /// Move section.
        /// key: moved section.
        /// value: SectionUnit.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_DistrictUnit> MoveDistrictUnit { get; set; }

        /// <summary>
        /// Attack enemis.
        /// value: SectionUnit.
        /// </summary>
        public ConcurrentDictionary<SectionId, World_SectionUnit> AttackSectionUnit { get; set; }

        public void Dispose()
        {
            MoveSectionUnit.Clear();
            MoveDistrictUnit.Clear();
        }
    }
}
