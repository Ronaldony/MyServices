using Akka.Actor;
using Server_Realm.Zones.HuntZone;

namespace Server_Realm.Core.ActorProviders
{
    /// <summary>
    /// HuntZoneProcessorProvider.
    /// </summary>
    public class HuntZoneProcessorProvider
    {
        private IActorRef _actorProvider;

        public HuntZoneProcessorProvider(ActorSystem actorSystem)
        {
            _actorProvider = actorSystem.ActorOf<HuntZoneProcessor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _actorProvider.Tell(data);
        }
    }
}
