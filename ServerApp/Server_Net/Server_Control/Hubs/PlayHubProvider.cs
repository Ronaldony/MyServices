using Akka.Actor;

namespace Server_Control.Hubs
{
    using Server_Control.Hubs.Play;

    /// <summary>
    /// PlayHubProvider.
    /// </summary>
    public class PlayHubProvider
    {
        private IActorRef _actor;

        public PlayHubProvider(IServiceProvider serviceProvider, ActorSystem actorSystem)
        {
            var props = PlayHub.Props(serviceProvider);
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
