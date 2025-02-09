namespace DataDesignerServer.Core.Http.Type
{
    /// <summary>
    /// REQ_GetAllType.
    /// </summary>
    [HttpAttribute("/type/get-all")]
    public class REQ_GetAllType
    {
    }

    /// <summary>
    /// RES_GetAllType.
    /// </summary>
    public class RES_GetAllType
    {
        /// <summary>
        /// SchemaNames.
        /// </summary>
        public List<string> SchemaNames { get; set; }
    }
}
