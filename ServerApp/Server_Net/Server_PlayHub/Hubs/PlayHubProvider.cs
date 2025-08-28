using Akka.Actor;

namespace Server_PlayHub.Hubs
{
    using Server_PlayHub.Actors.Post;

    /// <summary>
    /// PlayHubProvider.
    /// </summary>
    public class PlayHubProvider
    {
        private IActorRef _actor;

        public PlayHubProvider(ActorSystem actorSystem)
        {
            _actor = actorSystem.ActorOf<PushPostActor>();
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
