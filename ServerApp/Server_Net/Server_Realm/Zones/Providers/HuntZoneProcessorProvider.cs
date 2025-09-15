using Akka.Actor;

namespace Server_Realm.Zones.Providers
{
    using Akka.Actor.Dsl;
    using Server_Realm.Zones.HuntZone;

    /// <summary>
    /// HuntZoneProcessorProvider.
    /// </summary>
    public class HuntZoneProcessorProvider
    {
        private IActorRef _actor;

        public HuntZoneProcessorProvider(IServiceProvider serviceProvider, ActorSystem actorSystem)
        {
            var props = HuntZoneProcessor.Props(serviceProvider);
            _actor = actorSystem.ActorOf(props);
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
