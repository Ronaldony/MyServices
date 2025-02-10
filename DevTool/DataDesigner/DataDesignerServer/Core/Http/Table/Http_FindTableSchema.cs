
namespace DataDesignerServer.Core.Http.Table
{
    using DataDesignerServer.Core.Data;

	/// <summary>
	/// REQ_FindTableSchema.
	/// </summary>
	[HttpAttribute("/table/find-schema")]
    public class REQ_FindTableSchema
	{

    }

	/// <summary>
	/// RES_FindTableSchema.
	/// </summary>
	public class RES_FindTableSchema
	{
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }
    }
}
