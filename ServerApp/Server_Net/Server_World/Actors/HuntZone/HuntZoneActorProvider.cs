using Akka.Actor;

namespace Server_World.Actors.HuntZone
{
    /// <summary>
    /// HuntZoneActorProvider.
    /// </summary>
    public class HuntZoneActorProvider
    {
        private IActorRef _huntzoneAcotr;

        public HuntZoneActorProvider(ActorSystem actorSystem)
        {
            _huntzoneAcotr = actorSystem.ActorOf<HuntZoneActor>();
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _huntzoneAcotr.Tell(data);
        }
    }
}
