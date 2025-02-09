using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.Http.Type
{
    /// <summary>
    /// REQ_FindTypeColumns.
    /// </summary>
    public class REQ_FindTypeColumns
    {
    }

    /// <summary>
    /// RES_FindTypeColumns.
    /// </summary>
    public class RES_FindTypeColumns
    {
        /// <summary>
        /// Columns.
        /// </summary>
        public List<TypeSchemaColumn> Columns { get; set; }
    }
}
