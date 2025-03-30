namespace Framwork.Core.Session
{
    /// <summary>
    /// GameSession.
    /// </summary>
    public class GameSession
    {
        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// SessionId.
        /// </summary>
        public int SessionId { get; set; }   

        /// <summary>
        /// SessionUid.
        /// </summary>
        public ulong SessionUid { get; set; }
    }
}
