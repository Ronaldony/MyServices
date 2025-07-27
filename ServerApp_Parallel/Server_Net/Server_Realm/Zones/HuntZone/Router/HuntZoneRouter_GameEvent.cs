using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_Realm.Zones.HuntZone.Router
{
    using Server_Realm.Core.Types;
    using Server_Realm.GameEvents;

    /// <summary>
    /// HuntZoneRouter.
    /// </summary>
    public partial class HuntZoneRouter
    {
        public ActionBlock<GameEvent> Initialize_GameEventPipeline(CancellationToken ct)
        {
            var pipeline = new ActionBlock<GameEvent>(
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,     // single thread.
                    CancellationToken = ct,         // cancel token.
                },
                action: ge =>
                {
                    switch (ge.GetGameEventType())
                    {
                        case Type_GameEvent.ReceivePacket:
                            {
                                // 위치한 섹션에 전파.
                                if (false == _playerDistrictDict.TryGetValue(ge.Pid, out var district))
                                {
                                    _logger.LogError($"not exist player. pid: {ge.Pid}");
                                    return;
                                }

                                // Write game event.
                                _districtProcessors[district].WriteGameEvent(ge);
                            }
                            break;
                        case Type_GameEvent.MoveDistrict:
                            {
                                var moveDistrictEvent = ge as MoveDistrictEvent;
                                var moveDistrict = GetDistrict(moveDistrictEvent.RouteSection);
                                
                                // 플레이어 구역 갱신.
                                foreach (var wp in moveDistrictEvent.PlayerUnits)
                                {
                                    _playerDistrictDict.AddOrUpdate(wp.Pid, moveDistrict, (_, _) => moveDistrict);
                                }

                                // Write game event.
                                var districts = GetAroundDistrict(moveDistrictEvent.RouteSection);

                                foreach (var district in districts)
                                {
                                    // 출처 구역을 제외한 주변 구역에 전파.
                                    if (district != moveDistrictEvent.FromDistrict)
                                    {
                                        _districtProcessors[district].WriteGameEvent(ge);
                                    }
                                }

                                // 처리 구역에 이벤트 전송.
                                _districtProcessors[GetDistrict(moveDistrictEvent.RouteSection)].WriteGameEvent(ge);
                            }
                            break;
                        default:
                            {
                                // 주변 구역에 전파.
                                var aroundDistricts = GetAroundDistrict(ge.RouteSection);

                                foreach (var sectionGroup in aroundDistricts)
                                {
                                    _districtProcessors[sectionGroup].WriteGameEvent(ge);
                                }
                            }
                            break;
                    }

                });

            return pipeline;
        }
    }
}
