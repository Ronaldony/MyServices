
namespace Framework.Core.Database.DBO
{
	using Dapper.ColumnMapper;
	using Framework.Core.Database.Attributes;

	[DapperMapping]
    public class DBO_User
	{
		/// <summary>
		/// 유저 로그인 Id
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
