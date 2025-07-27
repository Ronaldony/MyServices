namespace Server_Realm.Zones.HuntZone.District
{
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Get interest section.
        /// </summary>
        private bool IsManagedSection(SectionId section)
        {
            return _worldGroup.ManagedSections.Contains(section);
        }
    }
}
