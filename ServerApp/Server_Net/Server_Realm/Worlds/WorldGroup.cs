using System.Collections.Concurrent;

namespace Server_Realm.Worlds
{
    using Framework.Core.Database.Interfaces;
    using Server_Realm.Context.Units;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;
    using Server_Realm.Worlds.Sections;

    /// <summary>
    /// WorldGroup.
    /// </summary>
    public class WorldGroup
    {
        /// <summary>
        /// Around section dictionary.
        /// key: source section.
        /// value: around sections.
        /// </summary>
        public Dictionary<SectionId, HashSet<SectionId>> AroundSectionDict;

        /// <summary>
        /// 섹션 단위 정보.
        /// key: section.
        /// value: world section.
        /// </summary>
        public Dictionary<SectionId, WorldSection> WorldSectionDict;

        /// <summary>
        /// Player units.
        /// key: Pid.
        /// value: PlayerUnit.
        /// </summary>
        public Dictionary<string, PlayerUnit> PlayerUnitDict;

        /// <summary>
        /// Mob units.
        /// key: UnitId.
        /// value: MobUnit.
        /// </summary>
        public Dictionary<long, MobUnit> MobUnitDict;

        /// <summary>
        /// DataObject dictionary.
        /// key: Pid.
        /// value: IDataObject.
        /// </summary>
        public Dictionary<string, List<IDataObject>> DataObjectDict;

        /// <summary>
        /// Reusable units.
        /// </summary>
        public ConcurrentQueue<Unit> ReusableUnits;
    }
}
