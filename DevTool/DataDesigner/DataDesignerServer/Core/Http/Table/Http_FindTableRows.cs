
namespace DataDesignerServer.Core.Http.Table
{
    using DataDesignerServer.Core.Data;

	/// <summary>
	/// REQ_FindTableRows.
	/// </summary>
	[HttpAttribute("/table/find-rows")]
    public class REQ_FindTableRows
    {

    }

	/// <summary>
	/// RES_FindTableRows.
	/// </summary>
	public class RES_FindTableRows
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }

        /// <summary>
        /// Rows.
        /// </summary>
        public List<object> Rows { get; set; }
    }
}
