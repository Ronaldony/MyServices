using System.Net;

namespace ServerEngine.Network.Interfaces
{
    public interface IUdpSession
    {
        /// <summary>
        /// Session id.
        /// </summary>
        public int SessionId { get; }

        /// <summary>
        /// Session address.
        /// </summary>
        public IPAddress Address { get; }
    }
}
