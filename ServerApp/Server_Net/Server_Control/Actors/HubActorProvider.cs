using Akka.Actor;

namespace Server_Control.Actors
{
    /// <summary>
    /// HubActorProvider.
    /// </summary>
    public class HubActorProvider
    {
        private IActorRef _actor;

        public HubActorProvider(IServiceProvider serviceProvider, ActorSystem actorSystem)
        {
            var props = HubActor.Props(serviceProvider);
            _actor = actorSystem.ActorOf(props);
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _actor.Tell(data);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            Tell("initialize");
        }
    }
}
