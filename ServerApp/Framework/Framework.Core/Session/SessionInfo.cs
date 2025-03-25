namespace Framwork.Core.Session
{
    /// <summary>
    /// GameSession.
    /// </summary>
    public partial class SessionInfo
    {
        /// <summary>
        /// 유저 Id.
        /// </summary>
        public string UserId { get; init; }

        /// <summary>
        /// Uid.
        /// </summary>
        public string Uid { get; init; }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; init; }

        /// <summary>
        /// IP Address.
        /// </summary>
        public string Address { get; init; }

        /// <summary>
        /// Uid.
        /// </summary>
        public string AccessToken { get; init; }

        /// <summary>
        /// Uid.
        /// </summary>
        public string RefreshToken { get; init; }

        /// <summary>
        /// Uid.
        /// </summary>
        public DateTime TokenExpireTime { get; init; }

        /// <summary>
        /// GameSession.
        /// </summary>
        public GameSession GameSession { get; set; }
    }
}
