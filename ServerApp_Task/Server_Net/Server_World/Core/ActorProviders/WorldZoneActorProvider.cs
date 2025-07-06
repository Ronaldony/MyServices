using Akka.Actor;

namespace Server_World.Core.ActorProviders
{
    using Server_World.Actors.HuntZone;

    /// <summary>
    /// WorldZoneActorProvider.
    /// </summary>
    public class WorldZoneActorProvider
    {
        private IActorRef _worldZoneActor;

        public WorldZoneActorProvider(ActorSystem actorSystem)
        {
            _worldZoneActor = actorSystem.ActorOf<WorldZoneActor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _worldZoneActor.Tell(data);
        }
    }
}
