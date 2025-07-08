using Akka.Actor;

namespace Server_World.Actors.HuntZone
{
    /// <summary>
    /// WorldZoneActorProvider.
    /// </summary>
    public class WorldActorProvider
    {
        private IActorRef _worldAcotr;

        public WorldActorProvider(ActorSystem actorSystem)
        {
            _worldAcotr = actorSystem.ActorOf<WorldZoneActor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _worldAcotr.Tell(data);
        }
    }
}
