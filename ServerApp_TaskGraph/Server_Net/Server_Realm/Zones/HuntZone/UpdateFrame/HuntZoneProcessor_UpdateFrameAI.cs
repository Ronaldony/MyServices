namespace Server_Realm.Zones.HuntZone
{
    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - AI.
        /// </summary>
        private void UpdateFrame_AI(double deltaTime)
        {
            // Mobs.
            Parallel.ForEach(_worldGroup.MobUnitDict.Values, mu =>
            {
                mu.AIBrain.Update(deltaTime);
            });
        }
    }
}
