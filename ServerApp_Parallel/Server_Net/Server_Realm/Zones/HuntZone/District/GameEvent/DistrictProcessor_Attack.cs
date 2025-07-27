namespace Server_Realm.Zones.HuntZone.District
{
    using Server_Realm.Entities;
    using Server_Realm.GameEvents;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Process AttackEvent.
        /// </summary>
        public void Proc_AttackMeleeEvent(AttackMeleeEvent gameEvent)
        {
            var total = _worldGroup.SectionMobDict.Values.Count();
            var enemyUnits = new List<Unit>(total);

            foreach (var pair in _worldGroup.SectionMobDict.Values)
            {
                enemyUnits.AddRange(pair.Values);
            }

            // 근정 공격.
            gameEvent.PlayerUnit.AttackMelee(gameEvent.Angle, _worldGroup.MobDict.Values);
        }
    }
}
