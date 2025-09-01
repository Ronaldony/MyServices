using Akka.Actor;

namespace Server_Realm.Actors
{
    using Server_Realm.Actors.Post;

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
