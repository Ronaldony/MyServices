using Dapper.ColumnMapper;

namespace Framework.Core.Database.DBO
{
    using Framework.Core.Database.Attributes;

	/// <summary>
	/// DBO_User.
	/// </summary>
    [DapperMapping]
    public class DBO_User
	{
		/// <summary>
		/// 유저 로그인 고유 Id (절대 불변)
		/// </summary>
		[ColumnMapping("user_id")]
		public string UserId { get; set; }

		/// <summary>
		/// 발급된 유저 고유Id
		/// </summary>
		[ColumnMapping("uid")]
		public string Uid { get; set; }

        /// <summary>
        /// 생성시간
        /// </summary>
        [ColumnMapping("regtime")]
		public DateTime RegTime { get; set; }
	}
}
