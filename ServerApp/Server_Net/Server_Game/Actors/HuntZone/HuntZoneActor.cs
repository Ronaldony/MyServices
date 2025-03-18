using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_Game.Actors.HuntZone
{
    using GameCore.Packet;
    using Server_Game.Core.Utilities;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public sealed class HuntZoneActor : ReceiveActor 
    {
        private readonly ILogger<HuntZoneActor> _logger;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            // Receive.
            Receive<Packet_Move>(packet =>
            {

            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new HuntZoneActor());
        }
    }
}
