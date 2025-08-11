using System.Numerics;

namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Worlds;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Check valid section.
        /// </summary>
        public bool IsValidSection(Vector2 pos)
        {
            var section = SectionId.ToSection(pos);

            if (_worldGroup.AroundSectionDict.ContainsKey(section))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get around sections.
        /// </summary>
        public HashSet<SectionId> GetAroundSections(SectionId section)
        {
            return _worldGroup.AroundSectionDict[section];
        }
    }
}
