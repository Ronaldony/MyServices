using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.Http.Configuration
{
	/// <summary>
	/// REQ_UpdateConfigSchema,
	/// </summary>
	[HttpAttribute("/config/update-schema")]
    public class REQ_UpdateConfigSchema
    {
        /// <summary>
        /// Schema.
        /// </summary>
        public ClassSchema Schema { get; set; }
    }

	/// <summary>
	/// RES_UpdateConfigSchema,
	/// </summary>
	public class RES_UpdateConfigSchema
	{
    }
}
