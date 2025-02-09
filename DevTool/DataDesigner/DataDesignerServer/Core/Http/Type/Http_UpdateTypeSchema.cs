using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.Http.Type
{
    /// <summary>
    /// REQ_UpdateTypeSchema,
    /// </summary>
    [HttpAttribute("/type/update-schema")]
    public class REQ_UpdateTypeSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public TypeSchema Schema { get; set; }
    }

    /// <summary>
    /// RES_UpdateTypeSchema,
    /// </summary>
    public class RES_UpdateTypeSchema
    {
    }
}
