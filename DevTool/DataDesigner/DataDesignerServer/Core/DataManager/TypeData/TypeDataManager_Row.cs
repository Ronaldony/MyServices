using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.DataManager.TypeData
{
    /// <summary>
    /// EnumManager.
    /// </summary>
    public partial class TypeDataManager
    {
        /// <summary>
        /// Update class members.
        /// </summary>
        public bool UpdateRows(string typeName, List<TypeSchemaRow> rows)
        {
            // Find type.
            var type = FindTypeOrDefault(typeName);
            if (null == type)
            {
                return false;
            }

            // Update builder.
            UpdateType(typeName, rows);

            // Write update rows to the file.
            if (false == FlushFiles(typeName))
            {
                return false;
            }

            return true;
        }
    }
}
