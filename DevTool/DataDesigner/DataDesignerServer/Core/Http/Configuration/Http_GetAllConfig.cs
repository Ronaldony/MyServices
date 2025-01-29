namespace DataDesignerServer.Core.Http.Configuration
{
    /// <summary>
    /// Response for GetConfigurations.
    /// </summary>
    [HttpAttribute("/config/get-all")]
    public class REQ_GetAllConfig
    {
    }

    /// <summary>
    /// Response for GetConfigurations.
    /// </summary>
    public class RES_GetAllConfig
    {
        public List<string> SchemaNames { get; set; }
    }
}
