using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SQLite;
using System.Collections;

namespace GameCore
{
    using GameCore.Interfaces;

    /// <summary>
    /// TDBService.
    /// </summary>
    public sealed class TDBService : ITDBService
	{
		private readonly ILogger<TDBService> _logger;
		private readonly Dictionary<string, SQLiteConnection> _dbConnectionDict;
		private readonly Hashtable _dataCacheHashTable;
        private readonly Lock _lock;

        private readonly string _rootPath;
		private readonly string _databasePath;

		public const string DB_PATH = "Database";
		public const string DB_FILES = "*.db";

		public TDBService(IServiceProvider serviceProvider)
		{
			_logger = serviceProvider.GetRequiredService<ILogger<TDBService>>();

			_dataCacheHashTable = new Hashtable();
			_dbConnectionDict = new Dictionary<string, SQLiteConnection>();
            _lock = new Lock();

            _rootPath = Path.GetDirectoryName(System.AppContext.BaseDirectory);
			_databasePath = Path.Combine(_rootPath, DB_PATH);
		}

		public bool Initialize()
		{
			var totalTable = 0;
			var filePaths = Directory.GetFiles(_databasePath, DB_FILES, SearchOption.AllDirectories);

			foreach (var filePath in filePaths)
			{
				// Type 무시.
				if (filePath.Contains("Type"))
				{
					continue;
				}

                var tableName = Path.GetFileNameWithoutExtension(filePath);
                var connection = new SQLiteConnection(filePath);

                _dbConnectionDict.Add(tableName, connection);

                totalTable++;
            }

            _logger.LogInformation($"TDBService initialzed. total: {totalTable}");

			return true;
		}

        /// <summary>
        /// Get by id.
        /// </summary>
		public T GetById<T>(int id) where T : class, ITableBase, new()
        {
            if (_dataCacheHashTable.ContainsKey(typeof(T).Name))
            {
                return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).GetById(id);
            }

            // Create table.
            CreateHashTable<T>();

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).GetById(id);
        }

		/// <summary>
		/// retreive first data from predicate
		/// </summary>
		public T First<T>(Func<T, bool> predExpr) where T : class, ITableBase, new()
        {
            if (_dataCacheHashTable.ContainsKey(typeof(T).Name))
            {
                return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).First(predExpr);
            }

            // Create table.
            CreateHashTable<T>();

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).First(predExpr);
        }

		/// <summary>
		/// retreive first data from predicate
		/// </summary>
		public T Last<T>(Func<T, bool> predExpr) where T : class, ITableBase, new()
        {
            if (_dataCacheHashTable.ContainsKey(typeof(T).Name))
            {
                return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Last(predExpr);
            }

            // Create table.
            CreateHashTable<T>();

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Last(predExpr);
        }

		/// <summary>
		/// select data from predicate
		/// </summary>
		public IEnumerable<T> Select<T>(Func<T, bool> predExpr) where T : class, ITableBase, new()
        {
            if (_dataCacheHashTable.ContainsKey(typeof(T).Name))
            {
                return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Select(predExpr);
            }

            // Create table.
            CreateHashTable<T>();

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Select(predExpr);
        }

        /// <summary>
        /// select all data
        /// </summary>
        public IEnumerable<T> SelectAll<T>() where T : class, ITableBase, new()
        {
            if (_dataCacheHashTable.ContainsKey(typeof(T).Name))
            {
                return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).SelectAll();
            }

            // Create table.
            CreateHashTable<T>();

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).SelectAll();
        }

        /// <summary>
        /// Count.
        /// </summary>
        public int Count<T>() where T : class, ITableBase, new()
        {
            if (_dataCacheHashTable.ContainsKey(typeof(T).Name))
            {
                return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Count();
            }

            // Create table.
            CreateHashTable<T>();

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Count();
        }

        /// <summary>
        /// Create hash table.
        /// </summary>
        private void CreateHashTable<T>() where T : class, ITableBase, new()
        {
            // Create table.
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    var datas = _dbConnectionDict[typeof(T).Name].Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }
        }
    }
}
