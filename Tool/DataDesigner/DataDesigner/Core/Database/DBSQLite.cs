using SQLite;

namespace DataDesigner.Core.DBGenerator
{
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
        /// <returns></returns>
        public void Create(Type dbType)
        {
            var filePath = Path.Combine(_basePath, $"{dbType.Name}.db");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var db = new SQLiteConnection(filePath);

            db.CreateTable(dbType);
        }

        /// <summary>
        /// Insert data.
        /// </summary>
        public bool Insert(Type dbType, Type row)
        {
            using var db = new SQLiteConnection(Path.Combine(_basePath, $"{dbType.Name}.db"));

            var rowAffected = db.Insert(row);

            if (rowAffected < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update row.
        /// </summary>
        public bool Update(Type row)
        {
            using var db = new SQLiteConnection(Path.Combine(_basePath, $"{row.Name}.db"));

            var rowAffected = db.Update(row);

            if (rowAffected < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete row.
        /// </summary>
        public bool Delete(Type row)
        {
            using var db = new SQLiteConnection(Path.Combine(_basePath, $"{row.Name}.db"));

            var rowAffected = db.Delete(row);

            if (rowAffected < 1)
            {
                return false;
            }

            return true;
        }
    }
}
