namespace Server_Realm.Zones.HuntZone
{
    using Server_Realm.Worlds;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 프레임 업데이트 - AI.
        /// </summary>
        private void UpdateFrame_AI(double deltaTime, World_Section worldSection)
        {
            foreach (var mu in worldSection.GetMobUnits())
            {
                mu.AIBrain.Update(deltaTime);
            }
        }
    }
}
