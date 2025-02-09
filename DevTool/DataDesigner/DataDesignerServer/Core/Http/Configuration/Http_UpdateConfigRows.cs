namespace DataDesignerServer.Core.Http.Configuration
{
    /// <summary>
    /// REQ_UpdateConfigRows.
    /// </summary>
    [HttpAttribute("/config/update-rows")]
    public class REQ_UpdateConfigRows
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
    /// RES_UpdateConfigRows.
    /// </summary>
    public class RES_UpdateConfigRows
    {
    }
}
