using Akka.Actor;

namespace Server_World.Actors.HuntZone
{
    /// <summary>
    /// WorldZoneActorProvider.
    /// </summary>
    public class WorldZoneActorProvider
    {
        private IActorRef _worldzoneAcotr;

        public WorldZoneActorProvider(ActorSystem actorSystem)
        {
            _worldzoneAcotr = actorSystem.ActorOf<WorldZoneActor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _worldzoneAcotr.Tell(data);
        }
    }
}
