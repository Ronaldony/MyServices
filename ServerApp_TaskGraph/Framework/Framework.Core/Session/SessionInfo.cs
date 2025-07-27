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
        public string UserId { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// IP Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        public DateTime TokenExpireTime { get; set; }
    }
}
