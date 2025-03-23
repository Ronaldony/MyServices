using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Message;
    using Server_World.Core.Utilities;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        private readonly IMemoryPoolService _memoryPoolService;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            _memoryPoolService = ServiceUtility.GetService<IMemoryPoolService>();

            ///////////////////////////////// Receive.

            // Initlaize.
            Receive<HuntZoneMsg_LoadPlayer>(packet => LoadPlayer(packet));

            // Move.
            Receive<NetPacketCS_MoveStart>(packet => MoveStart(packet));
            Receive<NetPacketCS_MoveEnd>(packet => MoveEnd(packet));

            // Interaction.
            Receive<NetPacketCS_Attack>(packet => Attack(packet));
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new HuntZoneActor());
        }
    }
}
