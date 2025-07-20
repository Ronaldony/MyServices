using System.Collections.Concurrent;

namespace Server_World.Actors.HuntZone.District
{
    using Server_World.Core.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Initialize world group.
        /// </summary>
        private World_Group Initialize_WorldGroup(HashSet<SectionId> aroundSections)
        {
            var sectionPlayerDict = new Dictionary<SectionId, ConcurrentDictionary<string, World_Player>>();
            var sectionEnemyDict = new Dictionary<SectionId, ConcurrentDictionary<ulong, World_Enemy>>();

            foreach (var section in aroundSections)
            {
                sectionPlayerDict.Add(section, new ConcurrentDictionary<string, World_Player>());
                sectionEnemyDict.Add(section, new ConcurrentDictionary<ulong, World_Enemy>());
            }

            return new World_Group
            {
                EnemyDict = new Dictionary<ulong, World_Enemy>(),
                PlayerDict = new Dictionary<string, World_Player>(),
                SectionPlayerDict = sectionPlayerDict,
                SectionEnemyDict = sectionEnemyDict,
                ManagedSections = aroundSections
            };
        }
    }
}
