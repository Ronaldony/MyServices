
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Proccessor
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Datas.GameEvents;
    using Server_World.Core.Worlds;

    /// <summary>
    /// PacketProccessor.
    /// </summary>
    public partial class PacketProccessor
    {
        private readonly ILogger<PacketProccessor> _logger;

        private readonly TransformBlock<World_PacketEvent, GameEvent> _packetBlocks;

        public PacketProccessor(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PacketProccessor>>();

            var workerCount = Environment.ProcessorCount / 2;

            if (workerCount == 0)
            {
                workerCount = 1;
            }

            _packetBlocks = new TransformBlock<World_PacketEvent, GameEvent>(
                transform: packetEvent => ProcessPacketEvent(packetEvent), 
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    EnsureOrdered = true,
                    MaxDegreeOfParallelism = workerCount,
                    CancellationToken = ct,
                }); 
        }

        /// <summary>
        /// ProcessPacketEvent.
        /// </summary>
        private GameEvent ProcessPacketEvent(World_PacketEvent packetEvent)
        {
            GameEvent gameEvent;

            // Filter packet code.
            switch (packetEvent.NetPacket.PacketCode)
            {
                case NetPacketCode.CS_MoveStart:
                    gameEvent = MoveStart(packetEvent.Player, packetEvent.NetPacket.Packet as NetPacketCS_MoveStart);
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
