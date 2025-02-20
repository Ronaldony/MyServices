using System.Net;

namespace ServerEngine.Network.Interfaces
{
    /// <summary>
    /// UDP session interface.
    /// </summary>
    public interface IUdpSession
    {
        /// <summary>
        /// Session id.
        /// </summary>
        int SessionId { get; }

        /// <summary>
        /// IP Address.
        /// </summary>
        IPAddress Address { get; }
    }
}
