namespace DataDesignerServer.Core.Http.Table
{
	/// <summary>
	/// REQ_UpdateTableRows.
	/// </summary>
	[HttpAttribute("/table/update-rows")]
    public class REQ_UpdateTableRows
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Rows.
        /// </summary>
        public List<object> Rows { get; set; }
    }

	/// <summary>
	/// RES_UpdateTableRows.
	/// </summary>
	public class RES_UpdateTableRows
	{
    }
}
