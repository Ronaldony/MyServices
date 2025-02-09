
namespace DataDesignerServer.Core.Http.Type
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_FindTypeRows.
    /// </summary>
    [HttpAttribute("/type/find-rows")]
    public class REQ_FindTypeRows
    {

    }

    /// <summary>
    /// RES_FindTypeRows.
    /// </summary>
    public class RES_FindTypeRows
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public TypeSchema Schema { get; set; }

        /// <summary>
        /// Rows.
        /// </summary>
        public List<object> Rows { get; set; }
    }
}
