namespace DataDesignerServer.Core.Http.Common
{
	/// <summary>
	/// REQ_FetchRefTables.
	/// </summary>
	public class REQ_FetchRefTables
	{
	}

	/// <summary>
	/// RES_FetchRefTables.
	/// </summary>
	public class RES_FetchRefTables
	{
		public List<string> Tables { get; set; }
	}
}
