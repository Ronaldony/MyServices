

namespace DataDesignerServer.Core.Generator
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DBGenerator;

    /// <summary>
    /// DBGenerator.
    /// </summary>
    public sealed class DBGenerator
    {
        public DBGenerator()
        {
        }

        /// <summary>
        /// Generate DB for class,
        /// </summary>
        public bool GenerateClass(string basePath, Type classType, ClassSchema classSchema, IEnumerable<object> rows)
        {
            var dbSQLite = new DBSQLite(basePath);

            dbSQLite.CreateClassTable(classType, classSchema.Columns);

            // Create index.
            foreach (var column in classSchema.Columns)
            {
                // Check index
                if (column.IsIndexed)
                {
                    dbSQLite.CreateIndex(classType, column.Name);
                }
            }

            // Insert datas.
            foreach (var row in rows)
            {
                if (false == dbSQLite.InsertClass(classType, row))
                {
                    // Insert failed.
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generate DB for class,
        /// </summary>
        public bool GenerateEnum(string basePath, Type enumType, List<TypeSchemaRow> rows)
        {
            var dbSQLite = new DBSQLite(basePath);

            dbSQLite.CreateTypeTable(enumType);

            // Insert datas.
            foreach (var row in rows)
            {
                if (false == dbSQLite.InsertEnum(enumType, row))
                {
                    // Insert failed.
                    return false;
                }
            }

            return true;
        }
    }
}
