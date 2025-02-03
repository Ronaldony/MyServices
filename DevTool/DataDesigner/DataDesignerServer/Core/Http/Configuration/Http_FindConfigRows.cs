
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
        /// Schema of configuration.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Rows of the schema.
        /// </summary>
        public List<object> Rows { get; set; }
    }
}
