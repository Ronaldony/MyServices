
namespace DataDesignerServer.Core.Http.Configuration
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_FindConfigRows.
    /// </summary>
    [HttpAttribute("/config/find-rows")]
    public class REQ_FindConfigRows
    {

    }

    /// <summary>
    /// RES_FindConfigRows.
    /// </summary>
    public class RES_FindConfigRows
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
