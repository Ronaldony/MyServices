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
        private async Task UpdateFrame_AIAsync(double deltaTime)
        {
            // Mobs.
            await Task.Run(() =>
            {
                Parallel.ForEach(_worldGroup.MobUnitDict.Values, mu =>
                {
                    mu.AIBrain.Update(deltaTime);
                });
            });
        }
    }
}
