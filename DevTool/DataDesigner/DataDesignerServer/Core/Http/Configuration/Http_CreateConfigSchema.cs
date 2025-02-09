using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.Http.Configuration
{
    /// <summary>
    /// Request - Create configuration schema,
    /// </summary>
    [HttpAttribute("/config/create-schema")]
    public class REQ_CreateConfigSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }
    }

    /// <summary>
    /// Response - Create configuration schema,
    /// </summary>
    public class RES_CreateConfigSchema
    {

    }
}
