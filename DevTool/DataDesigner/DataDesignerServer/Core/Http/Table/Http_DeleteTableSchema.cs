namespace DataDesignerServer.Core.Http.Table
{
	/// <summary>
	/// REQ_DeleteTableSchema,
	/// </summary>
	[HttpAttribute("/table/delete-schema")]
    public class REQ_DeleteTableSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public string Schema { get; set; }
    }

	/// <summary>
	/// RES_DeleteTableSchema,
	/// </summary>
	public class RES_DeleteTableSchema
	{
    }
}
