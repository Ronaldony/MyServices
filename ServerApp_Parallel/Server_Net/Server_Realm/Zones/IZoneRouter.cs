namespace Server_Realm.Zones
{
    using Server_Realm.GameEvents;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// IZoneRouter.
    /// </summary>
    public interface IZoneRouter
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get around sections.
        /// </summary>
        HashSet<SectionId> GetAroundSections(SectionId section);

        /// <summary>
        /// Get around sections.
        /// </summary>
        HashSet<District> GetAroundDistrict(SectionId section);

        /// <summary>
        /// Get district.
        /// </summary>
        District GetDistrict(SectionId section);

        /// <summary>
        /// Validate section.
        /// </summary>
        bool IsValidSection(SectionId section);

        /// <summary>
        /// Route world packet.
        /// </summary>
        bool RouteGameEvent(GameEvent gameEvent);

        /// <summary>
        /// Route world message.
        /// </summary>
        public bool RouteWorldMsg(WorldMsg worldMsg);

        /// <summary>
        /// Broadcast packet around section.
        /// </summary>
        public void BroadcastPacketAround(BroadcastPacketEvent packetEvent);
    }
}
