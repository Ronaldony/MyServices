namespace DataDesignerServer.Core.Http.Configuration
{
	/// <summary>
	/// REQ_DeleteConfigSchema,
	/// </summary>
	[HttpAttribute("/config/delete-schema")]
    public class REQ_DeleteConfigSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public string Schema { get; set; }
    }

	/// <summary>
	/// RES_DeleteConfigSchema,
	/// </summary>
	public class RES_DeleteConfigSchema
	{
    }
}
