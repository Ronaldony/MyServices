using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.Http.Table
{
	/// <summary>
	/// REQ_UpdateTableSchema,
	/// </summary>
	[HttpAttribute("/table/update-schema")]
    public class REQ_UpdateTableSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }
    }

	/// <summary>
	/// RES_UpdateTableSchema,
	/// </summary>
	public class RES_UpdateTableSchema
	{
    }
}
