using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using Server_World.Core.Datas;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Types;

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
                    switch (ge.GameEventType)
                    {
                        case Type_GameEvent.ReceivePacket:
                            {
                                // 위치한 섹션에 전파.
                                if (false == _playerSectionDict.TryGetValue(ge.Pid, out var section))
                                {
                                    _logger.LogError($"not exist player. pid: {ge.Pid}");
                                    return;
                                }

                                // Write game event.
                                _districtProcessors[GetDistrict(section)].WriteGameEvent(ge);
                            }
                            break;
                        case Type_GameEvent.MoveDistrict:
                            {
                                var moveDistrictEvent = ge as MoveDistrictEvent;

                                // Write game event.
                                var districts = GetAroundDistrict(moveDistrictEvent.FromSection);

                                foreach (var district in districts)
                                {
                                    // 출처 구역을 제외한 주변 구역에 전파.
                                    if (district != moveDistrictEvent.FromDistrict)
                                    {
                                        _districtProcessors[district].WriteGameEvent(ge);
                                    }
                                }

                                // 처리 구역에 이벤트 전송.
                                _districtProcessors[GetDistrict(moveDistrictEvent.FromSection)].WriteGameEvent(ge);
                            }
                            break;
                        default:
                            {
                                // 주변 구역에 전파.
                                var aroundDistricts = GetAroundDistrict(ge.FromSection);

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
