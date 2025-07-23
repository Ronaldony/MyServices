using System.Collections.Concurrent;

namespace Server_World.Zones.HuntZone.District
{
    using Server_World.Core.Entities;
    using Server_World.Core.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Initialize world group.
        /// </summary>
        private World_Group Initialize_WorldGroup(HashSet<SectionId> managedSections)
        {
            var sectionPlayerDict = new Dictionary<SectionId, ConcurrentDictionary<string, PlayerEntity>>();
            var sectionEnemyDict = new Dictionary<SectionId, ConcurrentDictionary<ulong, EnemyEntity>>();

            foreach (var section in managedSections)
            {
                sectionPlayerDict.Add(section, new ConcurrentDictionary<string, PlayerEntity>());
                sectionEnemyDict.Add(section, new ConcurrentDictionary<ulong, EnemyEntity>());
            }

            return new World_Group
            {
                PlayerDict = new Dictionary<string, PlayerEntity>(),
                SectionPlayerDict = sectionPlayerDict,
                SectionEnemyDict = sectionEnemyDict,
                ManagedSections = managedSections
            };
        }
    }
}
