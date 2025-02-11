

namespace DataDesignerServer.Core.Generator
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DBGenerator;

    /// <summary>
    /// DBGenerator.
    /// </summary>
    public sealed class DBGenerator
    {
        private readonly IServiceProvider _serviceProvider;

        public DBGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Generate DB for class,
        /// </summary>
        public bool GenerateClass(string basePath, Type classType, List<Type> classList)
        {
            var dbSQLite = new DBSQLite(basePath);

            dbSQLite.Create(classType);

            foreach (var item in classList)
            {
                if (false == dbSQLite.Insert(classType, item))
                {
                    // Insert failed.
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generate DB for enum.
        /// </summary>
        public bool GenerateEnum(string basePath, Type enumType, List<TypeSchemaRow> rows)
        {
            var dbSQLite = new DBSQLite(basePath);

            dbSQLite.Create(enumType);

            foreach (var item in rows)
            {
                if (false == dbSQLite.Insert(enumType, item.GetType()))
                {
                    // Insert failed.
                    return false;
                }
            }

            return default;
        }
    }
}
