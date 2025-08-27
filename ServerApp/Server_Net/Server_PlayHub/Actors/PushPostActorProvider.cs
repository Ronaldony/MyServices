using Akka.Actor;
using Server_Realm.Actors.Post;

namespace Server_Realm.Actors
{
    /// <summary>
    /// PushPostActorProvider.
    /// </summary>
    public class PushPostActorProvider
    {
        private IActorRef _actor;

        public PushPostActorProvider(ActorSystem actorSystem)
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
