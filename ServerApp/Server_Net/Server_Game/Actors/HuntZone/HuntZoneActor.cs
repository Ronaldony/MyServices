using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Server_Game.Actors.HuntZone
{
    using GameCore.Packet;
    using Server_Game.Core.Utilities;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public class HuntZoneActor : ReceiveActor
    {
        private readonly ILogger<HuntZoneActor> _logger;

        public HuntZoneActor()
        {
            _logger = ServiceUtility.GetService<ILogger<HuntZoneActor>>();

            // Receive.
            Receive<Packet_Move>(packet => Move(packet));
            Receive<Packet_Attack>(packet => Attack(packet));
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new HuntZoneActor());
        }
        
        /////////////////////////////////////////////////////////////////////////
        /// 패키 처리.
        
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        public void Move(Packet_Move packet)
        {

        }

        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        public void Attack(Packet_Attack packet)
        {

        }
    }
}
