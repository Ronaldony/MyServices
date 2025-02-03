
namespace DataDesignerServer.Core.Http.Configuration
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_FindConfigSchema.
    /// </summary>
    [HttpAttribute("/config/find-schema")]
    public class REQ_FindConfigSchema
    {

    }

    /// <summary>
    /// RES_FindConfigSchema.
    /// </summary>
    public class RES_FindConfigSchema
    {
        public ClassSchema Schema { get; set; }
    }
}
