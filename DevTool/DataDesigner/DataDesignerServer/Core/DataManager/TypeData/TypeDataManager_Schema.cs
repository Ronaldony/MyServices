
namespace DataDesignerServer.Core.DataManager.TypeData
{
    using DataDesignerServer.Core.Data;

    /// <summary>
    /// TypeDataManager.
    /// </summary>
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

            // Update type group.
            AddOrUpdateTypeGroup(schemaName, schema: schema);

            // EnumBuilder.
            var rows = new List<TypeSchemaRow>();
            UpdateType(schemaName, rows);
            
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

            // Update type group.
            AddOrUpdateTypeGroup(schemaName, schema: schema);

            // Refresh schema file.
            if (false == FlushFiles(schemaName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create schema for class.
        /// </summary>
        public bool DeleteSchema(string schemaName)
        {
            // Old schema.
            var oldSchema = FindSchemaOrDefault(schemaName);
            if (null == oldSchema)
            {
                return false;
            }

            // remove type group.
            if (false == RemoveTypeGroup(schemaName))
            {
                return false;
            }

            // remove builder
            RemoveType(schemaName);

            // Refresh schema file.
            RemoveFiles(schemaName);

            return true;
        }
    }
}
