using Akka.Actor;

namespace Server_World.Core.ActorProviders
{
    using Server_World.Actors.HuntZone;

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
