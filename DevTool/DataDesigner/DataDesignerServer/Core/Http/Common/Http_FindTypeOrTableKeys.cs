namespace DataDesignerServer.Core.Http.Common
{
	/// <summary>
	/// REQ_FindTypeOrTableKeys.
	/// </summary>
	[HttpAttribute("/common/find-type-table-keys")]
	public class REQ_FindTypeOrTableKeys
	{

	}

	/// <summary>
	/// RES_FindTypeOrTableKeys.
	/// </summary>
	public class RES_FindTypeOrTableKeys
	{
		public List<string> Keys { get; set; }
	}
}
