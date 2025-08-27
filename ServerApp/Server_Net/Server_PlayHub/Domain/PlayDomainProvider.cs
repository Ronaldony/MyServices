using Akka.Actor;

namespace Server_Realm.Domain
{
    using Server_Realm.Actors.Post;

    /// <summary>
    /// PlayDomainProvider.
    /// </summary>
    public class PlayDomainProvider
    {
        private IActorRef _actor;

        public PlayDomainProvider(ActorSystem actorSystem)
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
