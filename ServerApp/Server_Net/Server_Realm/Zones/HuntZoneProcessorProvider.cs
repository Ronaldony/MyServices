using Akka.Actor;
using Server_Realm.Zones.HuntZone;

namespace Server_Realm.Zones
{
    /// <summary>
    /// HuntZoneProcessorProvider.
    /// </summary>
    public class HuntZoneProcessorProvider
    {
        private IActorRef _actor;

        public HuntZoneProcessorProvider(ActorSystem actorSystem)
        {
            _actor = actorSystem.ActorOf<HuntZoneProcessor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _actor.Tell(data);
        }
    }
}
