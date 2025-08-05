namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;
    using Server_Realm.Units.Mob;
    using Server_Realm.Worlds;

    /// <summary>
    /// WorldMsg_SpawnMob.
    /// </summary>
    public class WorldMsg_SpawnMob : WorldMsgBase
    {
        public WorldMsg_SpawnMob()
            : base(Type_WorldMsg.SpawnEnemy)
        {
            MobUnits = new Dictionary<SectionId, List<MobUnit>>();
        }

        /// <summary>
        /// Enemies.
        /// </summary>
        public Dictionary<SectionId, List<MobUnit>> MobUnits;
    }
}
