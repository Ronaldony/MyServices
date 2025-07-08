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
        public ActionBlock<GameEvent> initialize_GameEventPipeline(CancellationToken ct)
        {
            var pipeline = new ActionBlock<GameEvent>(
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = ct,
                },
                action: ge =>
                {
                    // Check pid.
                    if (false == _playerSectionGroupDict.TryGetValue(ge.Pid, out var sectionGroup))
                    {
                        _logger.LogError($"not exist player. pid: {ge.Pid}");
                        return;
                    }
                    
                    // Write game event.
                    _gameProccessors[sectionGroup].WriteGameEvent(ge);
                });

            return pipeline;
        }
    }
}
