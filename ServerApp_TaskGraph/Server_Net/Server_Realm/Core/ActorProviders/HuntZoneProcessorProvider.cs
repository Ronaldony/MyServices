using Akka.Actor;
using Server_Realm.Zones.HuntZone;

namespace Server_Realm.Core.ActorProviders
{
    /// <summary>
    /// HuntZoneProcessorProvider.
    /// </summary>
    public class HuntZoneProcessorProvider
    {
        private IActorRef _huntZoneProvider;

        public HuntZoneProcessorProvider(ActorSystem actorSystem)
        {
            _huntZoneProvider = actorSystem.ActorOf<HuntZoneProcessor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _huntZoneProvider.Tell(data);
        }
    }
}
