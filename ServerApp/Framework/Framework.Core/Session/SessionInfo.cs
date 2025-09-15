
using Framework.Core.Database.Interfaces;

namespace Framwork.Core.Session
{
    /// <summary>
    /// GameSession.
    /// </summary>
    public partial class SessionInfo
    {
        public SessionInfo()
        {

        }

        /// <summary>
        /// 유저 Id.
        /// </summary>
        public string UserId;

        /// <summary>
        /// Uid.
        /// </summary>
        public string Uid;

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// IP Address.
        /// </summary>
        public string Address;

        /// <summary>
        /// Uid.
        /// </summary>
        public string AccessToken;

        /// <summary>
        /// Uid.
        /// </summary>
        public string RefreshToken;

        /// <summary>
        /// Uid.
        /// </summary>
        public DateTime TokenExpireTime;
    }
}
