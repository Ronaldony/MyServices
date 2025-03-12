namespace GameCore
{
	/// <summary>
	/// SQLite config table
	/// </summary>
	public class ConfigTableBase
	{
		/// <summary>
		/// TC 테이블 명
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// JsonData
		/// </summary>
		public string JsonData { get; set; }
	}
}
