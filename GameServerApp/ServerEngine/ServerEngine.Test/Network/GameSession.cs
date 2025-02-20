
namespace ServerEngine.Test.Network
{
    using ServerEngine.Network.UDP;

    /// <summary>
    /// GameSession.
    /// </summary>
    public class GameSession : UdpSession
    {
        /// <summary>
        /// Uid.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }
    }
}
