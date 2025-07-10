using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Types;

    /// <summary>
    /// SectionUpstreamer.
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
                                _districtProcessors[GetSectionGroup(section)].WriteGameEvent(ge);
                            }
                            break;
                        default:
                            {
                                // 주변 섹션에 전파.
                                var aroundSectionGroups = GetAroundSectionGroup(ge.FromSection);

                                foreach (var sectionGroup in aroundSectionGroups)
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
