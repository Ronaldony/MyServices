
namespace DataDesignerServer.Core.Http.Enum
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
        public List<EnumSchemaRow> Rows { get; set; }
    }
}
