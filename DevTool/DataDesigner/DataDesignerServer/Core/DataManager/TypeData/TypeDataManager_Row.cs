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
        public bool UpdateRows(string enumName, List<TypeSchemaRow> rows)
        {
            // Find type.
            var type = GetEnumType(enumName);
            if (null == type)
            {
                return false;
            }

            // Update builder.
            UpdateEnumBuiler(enumName, rows);

            // Write update rows to the file.
            if (false == FlushFiles(enumName))
            {
                return false;
            }

            return true;
        }
    }
}
