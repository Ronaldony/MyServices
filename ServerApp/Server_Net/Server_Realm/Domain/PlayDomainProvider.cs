using Akka.Actor;
using Server_Realm.Actors.Post;

namespace Server_Realm.Domain
{
    /// <summary>
    /// PlayDomainProvider.
    /// </summary>
    public class PlayDomainProvider
    {
        private IActorRef _actor;

        public PlayDomainProvider(ActorSystem actorSystem)
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
