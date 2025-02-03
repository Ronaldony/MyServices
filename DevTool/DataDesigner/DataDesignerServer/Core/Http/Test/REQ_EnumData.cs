namespace DataDesignerServer.Core.Http.Test
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// REQ_EnumData.
    /// </summary>
    public class REQ_EnumData
    {
        /// <summary>
        /// EnumName.
        /// </summary>
        public string EnumName { get; set; }

        /// <summary>
        /// Rows.
        /// </summary>
        public List<TypeSchemaRow> Rows { get; set; }
    }
}
