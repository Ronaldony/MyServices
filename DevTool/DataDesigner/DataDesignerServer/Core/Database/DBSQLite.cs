using System.Text;
using System.Data;

namespace DataDesignerServer.Core.DBGenerator
{
    using DataDesignerServer.Core.Data;
    using SQLite;

    /// <summary>
    /// DBSQLite.
    /// </summary>
    public sealed class DBSQLite
    {
        private readonly string _basePath;

        public DBSQLite(string basePath)
        {
            _basePath = basePath;
        }

        /// <summary>
        /// Create table.
        /// </summary>
        public void CreateClassTable(Type dbType, IEnumerable<ClassSchemaColumn> columns)
        {
            var filePath = Path.Combine(_basePath, $"{dbType.Name}.db");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Create query.
            var sb = new StringBuilder();

            sb.AppendLine($"CREATE TABLE {dbType.Name} (");
            sb.AppendLine("Id INTEGER PRIMARY KEY,");

            // create query.
            foreach (var column in columns.Skip(1))
            {
                // 타입 변환.
                var type = column.Type switch
                {
                    "int" or "bool" or "long" => "INTEGER",                 // int
                    "float" or "double" or "string" => "TEXT",              // Text
                    "date" => "DATE",                                       // DateTime
                    _ => "TEXT"                                             // otherwise
                };

                // 마지막 컬럼인 경우 "," 없음.
                if (column.Equals(columns.Last()))
                {
                    sb.AppendLine($"{column.Name} {type}");
                }
                else
                {
                    sb.AppendLine($"{column.Name} {type},");
                }
            }

            sb.AppendLine(");");

            // Create table.
            using var db = new SQLiteConnection(filePath);

            db.Execute(sb.ToString());
        }

        /// <summary>
        /// Create type table.
        /// </summary>
        public void CreateTypeTable(Type dbType)
        {
            var filePath = Path.Combine(_basePath, $"{dbType.Name}.db");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Create table.
            using var db = new SQLiteConnection(filePath);

            db.CreateTable(typeof(TypeSchemaRow));
        }

        /// <summary>
        /// Create table.
        /// </summary>
        /// <returns></returns>
        public bool CreateIndex(Type dbType, string columnName)
        {
            var filePath = Path.Combine(_basePath, $"{dbType.Name}.db");
            if (false == File.Exists(filePath))
            {
                return false;
            }

            // Create index.
            using var db = new SQLiteConnection(filePath);

            db.CreateIndex(dbType.Name, columnName);

            return true;
        }

        /// <summary>
        /// Insert data.
        /// </summary>
        public bool Insert(Type dbType, object row)
        {
            using var db = new SQLiteConnection(Path.Combine(_basePath, $"{dbType.Name}.db"));

            var rowAffected = db.Insert(row);

            if (rowAffected < 1)
            {
                return false;
            }

            return true;
        }
    }
}
