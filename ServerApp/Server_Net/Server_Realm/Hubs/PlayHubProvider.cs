using Akka.Actor;

namespace Server_Realm.Hubs
{
    using Server_Realm.Hubs.Play;

    /// <summary>
    /// PlayHubProvider.
    /// </summary>
    public class PlayHubProvider
    {
        private IActorRef _actor;

        public PlayHubProvider(ActorSystem actorSystem)
        {
            _actor = actorSystem.ActorOf <PlayHub>();
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
