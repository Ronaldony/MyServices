using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using Server_World.Core.Datas.GameEvents;

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
                    // Check pid.
                    if (false == _playerSectionDict.TryGetValue(ge.Pid, out var section))
                    {
                        _logger.LogError($"not exist player. pid: {ge.Pid}");
                        return;
                    }

                    // Write game event.
                    _districtProcessors[GetSectionGroup(section)].WriteGameEvent(ge);
                });

            return pipeline;
        }
    }
}
