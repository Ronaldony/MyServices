using Akka.Actor;

namespace Framework.Core.Database.Actors
{
    /// <summary>
    /// DataInfoSaveActor.
    /// </summary>
    public class DataInfoSaveActorProvider
    {
        private IActorRef _actor;

        public DataInfoSaveActorProvider(IServiceProvider serviceProvider, ActorSystem actorSystem)
        {
            var props = DataInfoSaveActor.Props(serviceProvider);
            _actor = actorSystem.ActorOf(props);
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
