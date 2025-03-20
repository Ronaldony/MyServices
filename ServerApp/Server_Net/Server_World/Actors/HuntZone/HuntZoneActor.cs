using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_World.Actors.HuntZone
{
    using GameCore.Packet;
    using Server_World.Core.Utilities;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            ///////////////////////////////// Receive.

            // Initlaize.
            Receive<NetPacketCS_Login>(packet => LoadPlayer(packet));

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
