using Akka.Actor;

namespace Server_Realm.Actors
{
    /// <summary>
    /// HubActorProvider.
    /// </summary>
    public class HubActorProvider
    {
        private IActorRef _actor;

        public HubActorProvider(ActorSystem actorSystem)
        {
            _actor = actorSystem.ActorOf<HubActor>();
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
