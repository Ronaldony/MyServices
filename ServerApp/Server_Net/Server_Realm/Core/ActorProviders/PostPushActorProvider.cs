using Akka.Actor;

namespace Server_Realm.Core.ActorProviders
{
    using Server_Realm.Actors.Post;

    /// <summary>
    /// PostPushActorProvider.
    /// </summary>
    public class PostPushActorProvider
    {
        private IActorRef _actorProvider;

        public PostPushActorProvider(ActorSystem actorSystem)
        {
            _actorProvider = actorSystem.ActorOf<PostPushActor>();
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
