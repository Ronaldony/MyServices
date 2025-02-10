
namespace DataDesignerServer.Core.Http.Table
{
	using DataDesignerServer.Core.Data;

	/// <summary>
	/// REQ_CreateTableSchema,
	/// </summary>
	[HttpAttribute("/table/create-schema")]
    public class REQ_CreateTableSchema
	{
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }
    }

	/// <summary>
	/// RES_CreateTableSchema,
	/// </summary>
	public class RES_CreateTableSchema
	{

    }
}
