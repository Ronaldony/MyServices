
namespace DataDesignerServer.Core.Http.Configuration
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_GetConfigSchema.
    /// </summary>
    [HttpAttribute("/config/get-schema-info")]
    public class REQ_GetConfigSchemaInfo
    {

    }

    /// <summary>
    /// RES_GetConfigSchemaInfo.
    /// </summary>
    public class RES_GetConfigSchemaInfo
    {
        public ClassSchema Schema { get; set; }
    }
}
