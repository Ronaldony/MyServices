using Akka.Actor;
using Server_Realm.Actors.Post;

namespace Server_Realm.Actors
{
    /// <summary>
    /// PostPushActorProvider.
    /// </summary>
    public class PostPushActorProvider
    {
        private IActorRef _actor;

        public PostPushActorProvider(ActorSystem actorSystem)
        {
            _actor = actorSystem.ActorOf<PostPushActor>();
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
