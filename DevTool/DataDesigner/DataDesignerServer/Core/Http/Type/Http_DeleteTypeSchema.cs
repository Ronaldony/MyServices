namespace DataDesignerServer.Core.Http.Type
{
    /// <summary>
    /// REQ_DeleteTypeSchema,
    /// </summary>
    [HttpAttribute("/type/delete-schema")]
    public class REQ_DeleteTypeSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public string Schema { get; set; }
    }

    /// <summary>
    /// RES_DeleteTypeSchema,
    /// </summary>
    public class RES_DeleteTypeSchema
    {
    }
}
