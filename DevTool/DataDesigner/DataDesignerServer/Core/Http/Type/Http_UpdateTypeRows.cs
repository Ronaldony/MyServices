namespace DataDesignerServer.Core.Http.Configuration
{
    /// <summary>
    /// REQ_UpdateTypeRows.
    /// </summary>
    [HttpAttribute("/type/update-rows")]
    public class REQ_UpdateTypeRows
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
    /// RES_UpdateTypeRows.
    /// </summary>
    public class RES_UpdateTypeRows
    {
    }
}
