
namespace DataDesignerServer.Core.Http.Type
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_CreateTypeSchema.
    /// </summary>
    [HttpAttribute("/type/create-schema")]
    public class REQ_CreateTypeSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public TypeSchema Schema { get; set; }
    }

    /// <summary>
    /// RES_CreateTypeSchema.
    /// </summary>
    public class RES_CreateTypeSchema
    {

    }
}
