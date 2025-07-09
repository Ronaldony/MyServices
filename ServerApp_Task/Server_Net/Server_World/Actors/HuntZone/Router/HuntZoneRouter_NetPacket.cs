using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using Server_World.Core.Datas.GameEvents;

    /// <summary>
    /// SectionUpstreamer.
    /// </summary>
    public partial class HuntZoneRouter
    {
        /// <summary>
        /// Initialize NetPacket.
        /// </summary>
        public ActionBlock<AroundPacketEvent> Initialize_NetPacketPipeline(CancellationToken ct)
        {
            var pipeline = new ActionBlock<AroundPacketEvent>(
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,     // single thread.
                    CancellationToken = ct,         // cancel token.
                },
                action: packetEvent =>
                {
                    var groups = _worldSectionGroup.AroundSectionGroupDict[packetEvent.Section];

                    // 주변 섹션 그룹에 전파.
                    foreach (var group in groups)
                    {
                        _gameProcessors[group].SendPacketAroundSections(packetEvent);
                    }
                });

            return pipeline;
        }
    }
}
