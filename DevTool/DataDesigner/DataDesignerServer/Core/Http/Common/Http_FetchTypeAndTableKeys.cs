namespace DataDesignerServer.Core.Http.Common
{
	/// <summary>
	/// REQ_FindTypeAndTableKeys.
	/// </summary>
	[HttpAttribute("/common/fetch-type-table-keys")]
	public class REQ_FetchTypeAndTableKeys
	{

	}

	/// <summary>
	/// RES_FindTypeAndTableKeys.
	/// </summary>
	public class RES_FetchTypeAndTableKeys
	{
		/// <summary>
		/// Key dictionary.
		/// key: schema name.
		/// value: keys.
		/// </summary>
		public Dictionary<string, List<string>> SchemaKeysDict { get; set; }
	}
}
