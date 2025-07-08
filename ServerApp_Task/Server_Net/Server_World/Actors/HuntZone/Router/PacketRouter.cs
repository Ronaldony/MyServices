
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Proccessor
{
    using GameCore;
    using Server_World.Actors.HuntZone.Processor;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;

    /// <summary>
    /// PacketRouter.
    /// </summary>
    public partial class PacketRouter
    {
        private readonly ILogger<PacketRouter> _logger;

        private readonly ActionBlock<World_NetPacket> _routePipeline;

        private readonly MoveProccessor _moveProccessor;

        public PacketRouter(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PacketRouter>>();

            _moveProccessor = new MoveProccessor();

            var workerCount = Environment.ProcessorCount / 2  == 0 ? 1 : Environment.ProcessorCount / 2;

            // event pipeline
            _routePipeline = new ActionBlock<World_NetPacket>(
                action: packet => RoutePacket(packet),
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = ct,
                });
        }

        /// <summary>
        /// ProcessPacketEvent.
        /// </summary>
        private GameEvent RoutePacket(World_NetPacket netPacket)
        {
            // Filter packet code.
            switch (netPacket.PacketCode)
            {
                case NetPacketCode.CS_MoveStart:
                    break;
                case NetPacketCode.CS_MoveEnd:
                    break;
                case NetPacketCode.CS_Attack:
                    break;
                case NetPacketCode.CS_KnockBack:
                    break;
                case NetPacketCode.CS_Dash:
                    break;
            }

            return default;
        }
    }
}
