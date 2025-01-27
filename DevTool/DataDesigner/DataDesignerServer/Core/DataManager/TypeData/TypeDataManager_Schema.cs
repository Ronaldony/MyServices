using DataDesignerServer.Core.Data;

namespace DataDesignerServer.Core.DataManager.TypeData
{
    public partial class TypeDataManager
    {
        /// <summary>
        /// Create schema.
        /// </summary>
        public bool CreateSchema(string schemaName, TypeSchema schema)
        {
            // Schema.
            var oldSchema = FindSchemaOrDefault(schemaName);
            if (null != oldSchema)
            {
                // Schema가 이미 있는 경우.
                return false;
            }

            // Schema.
            if (default == SetSchema(schemaName, schema))
            {
                return false;
            }

            // EnumBuilder.
            var rows = new List<TypeSchemaRow>();
            var eb = CreateTypeBuilder(schemaName, rows);
            
            // Set rows, builder.
            SetRows(schemaName, rows);
            SetTypeBuilder(schemaName, eb);

            // Flush.
            if (false == FlushFiles(schemaName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create schema for class.
        /// </summary>
        public bool UpdateSchema(string schemaName, TypeSchema schema)
        {
            // Old schema.
            var oldSchema = FindSchemaOrDefault(schemaName);
            if (null == oldSchema)
            {
                return false;
            }

            // Set schema.
            if (default == SetSchema(schemaName, schema))
            {
                return false;
            }

            // Refresh schema file.
            if (false == FlushFiles(schemaName))
            {
                return false;
            }

            return true;
        }
    }
}
