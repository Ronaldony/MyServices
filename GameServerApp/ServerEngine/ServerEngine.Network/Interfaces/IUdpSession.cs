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
        /// Disconnction check.
        /// </summary>
        public ulong SessionUid { get; set; }

        /// <summary>
        /// Disconnction check.
        /// </summary>
        public bool IsDisconn { get; set; }

        /// <summary>
        /// IP Address.
        /// </summary>
        IPAddress Address { get; }
    }
}
