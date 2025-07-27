
using Dapper.ColumnMapper;

namespace Framework.Core.Database.DBO
{
    using Framework.Core.Database.Attributes;

	/// <summary>
	/// DBO_Identity.
	/// </summary>
    [DapperMapping]
    public class DBO_Identity
	{
		/// <summary>
		/// 발급된 유저 고유 Id
		/// </summary>
		[ColumnMapping("uid")]
		public string Uid { get; set; }

        /// <summary>
        /// 발급된 플레이어 고유 Id
        /// </summary>
        [ColumnMapping("pid")]
        public string Pid { get; set; }

        /// <summary>
        /// Access token.
        /// </summary>
        [ColumnMapping("token")]
		public string Token { get; set; }

		/// <summary>
		/// RefreshToken
		/// </summary>
		[ColumnMapping("refresh_token")]
		public string RefreshToken { get; set; }

        /// <summary>
        /// 생성시간
        /// </summary>
        [ColumnMapping("token_expire")]
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 생성시간
        /// </summary>
        [ColumnMapping("regtime")]
		public DateTime RegTime { get; set; }
	}
}
