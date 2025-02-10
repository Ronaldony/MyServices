namespace DataDesignerServer.Core.Http.Table
{
	/// <summary>
	/// REQ_GetAllTable.
	/// </summary>
	[HttpAttribute("/table/get-all")]
    public class REQ_GetAllTable
    {
    }

	/// <summary>
	/// RES_GetAllTable.
	/// </summary>
	public class RES_GetAllTable
    {
        /// <summary>
        /// SchemaNames.
        /// </summary>
        public List<string> SchemaNames { get; set; }
    }
}
