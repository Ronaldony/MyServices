using Akka.Actor;

namespace ServerEngine.Network.Actors.Udp
{
    /// <summary>
    /// UdpSendActorProvider.
    /// </summary>
    public class UdpSendActorProvider
    {
        private IActorRef _udpSendActorRef;

        public UdpSendActorProvider(IActorRef actorRef)
        {
            _udpSendActorRef = actorRef;
        }

        /// <summary>
        /// Tell data.
        /// </summary>
        public void Tell<T>(T data)
        {
            _udpSendActorRef.Tell(data);
        }
    }
}
