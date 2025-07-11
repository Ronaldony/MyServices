using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using Server_World.Core.Datas.GameEvents;

    /// <summary>
    /// HuntZoneRouter.
    /// </summary>
    public partial class HuntZoneRouter
    {
        /// <summary>
        /// Initialize NetPacket.
        /// </summary>
        public ActionBlock<BroadcastPacketEvent> Initialize_NetPacketPipeline(CancellationToken ct)
        {
            var pipeline = new ActionBlock<BroadcastPacketEvent>(
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,     // single thread.
                    CancellationToken = ct,         // cancel token.
                },
                action: packetEvent =>
                {
                    var groups = _worldSectionGroup.AroundDistrictDict[packetEvent.FromSection];

                    // 주변 구역에 전파.
                    foreach (var group in groups)
                    {
                        _districtProcessors[group].WriteGameEvent(packetEvent);
                    }
                });

            return pipeline;
        }
    }
}
