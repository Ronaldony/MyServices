
namespace Server_World.Actors.HuntZone.District
{
    using Server_World.Core.Datas.GameEvents;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// Process MoveDistrictEvent.
        /// </summary>
        public void Proc_MoveDistrictEvent(MoveDistrictEvent moveDistrictEvent)
        {
            // TODO: 현재 구역에 이동하였는지 검사.

            // TODO: 현재 구역인 경우, 유닛 리스트에 추가 및 새로운 시야 전송.
            // TODO: 현재 구역이 아닌 경우, 새로운 시야 전송.
        }
    }
}
