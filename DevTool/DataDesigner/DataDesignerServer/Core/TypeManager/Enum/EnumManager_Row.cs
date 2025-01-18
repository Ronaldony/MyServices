using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.TypeManager.Enum
{
    /// <summary>
    /// EnumManager.
    /// </summary>
    public partial class EnumManager
    {
        /// <summary>
        /// Update class members.
        /// </summary>
        public bool UpdateRows(string enumName, List<EnumSchemaRow> rows)
        {
            // Find type.
            var type = GetEnumType(enumName);
            if (null == type)
            {
                return false;
            }

            // Create builder.
            var eb = CreateEnumBuilder(enumName, rows);

            // Set row, builder.
            SetEnumBuilder(enumName, eb);
            SetRows(enumName, rows);

            // Write update rows to the file.
            if (false == FlushFiles(enumName))
            {
                return false;
            }

            return true;
        }
    }
}
