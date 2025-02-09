
namespace DataDesignerServer.Core.Http.Configuration
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_FindTypeSchema.
    /// </summary>
    [HttpAttribute("/type/find-schema")]
    public class REQ_FindTypeSchema
    {

    }

    /// <summary>
    /// RES_FindTypeSchema.
    /// </summary>
    public class RES_FindTypeSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public TypeSchema Schema { get; set; }
    }
}
