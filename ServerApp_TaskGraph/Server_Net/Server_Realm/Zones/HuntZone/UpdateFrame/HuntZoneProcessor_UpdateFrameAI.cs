
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
        private void UpdateFrame_AI(double deltaTime, List<SectionId> sections)
        {
            foreach (var section in sections)
            {
                var ws = _worldGroup.WorldSectionDict[section];

                // 몹 투사체와 플레이어 충돌 처리.
                foreach (var mu in ws.GetMobUnits())
                {
                    mu.AIBrain.Update(deltaTime);
                }
            }
        }
    }
}
