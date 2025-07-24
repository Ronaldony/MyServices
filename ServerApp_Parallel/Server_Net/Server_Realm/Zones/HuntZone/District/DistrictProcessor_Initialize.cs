using System.Collections.Concurrent;

namespace Server_Realm.Zones.HuntZone.District
{
    using Server_Realm.Entities;
    using Server_Realm.Worlds;

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
            var sectionPlayerDict = new Dictionary<SectionId, ConcurrentDictionary<string, PlayerUnit>>();
            var sectionEnemyDict = new Dictionary<SectionId, ConcurrentDictionary<ulong, EnemyUnit>>();

            foreach (var section in managedSections)
            {
                sectionPlayerDict.Add(section, new ConcurrentDictionary<string, PlayerUnit>());
                sectionEnemyDict.Add(section, new ConcurrentDictionary<ulong, EnemyUnit>());
            }

            return new World_Group
            {
                PlayerDict = new Dictionary<string, PlayerUnit>(),
                SectionPlayerDict = sectionPlayerDict,
                SectionEnemyDict = sectionEnemyDict,
                ManagedSections = managedSections
            };
        }
    }
}
