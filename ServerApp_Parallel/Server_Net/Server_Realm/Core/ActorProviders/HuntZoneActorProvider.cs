using Akka.Actor;

namespace Server_Realm.Core.ActorProviders
{
    using Server_Realm.Zones.HuntZone;

    /// <summary>
    /// WorldZoneActorProvider.
    /// </summary>
    public class HuntZoneActorProvider
    {
        private IActorRef _huntZoneProvider;

        public HuntZoneActorProvider(ActorSystem actorSystem)
        {
            _huntZoneProvider = actorSystem.ActorOf<HuntZoneActor>();
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
