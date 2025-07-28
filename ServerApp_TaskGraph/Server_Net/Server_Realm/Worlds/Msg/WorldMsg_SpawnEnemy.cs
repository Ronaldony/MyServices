namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;
    using Server_Realm.Units.Enemy;
    using Server_Realm.Worlds;

    /// <summary>
    /// WorldMsg_SpawnEnemy.
    /// </summary>
    public class WorldMsg_SpawnEnemy : WorldMsgBase
    {
        public WorldMsg_SpawnEnemy()
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
