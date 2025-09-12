using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// TDBService.
    /// </summary>
    public class TDBManagement : Singleton<TDBManagement>
	{
		private readonly Dictionary<string, string> _dbPathDict;
		private readonly Hashtable _dataCacheHashTable;
        private readonly object _lock;

		private readonly string _databasePath;

		public const string DB_PATH = "Database";
		public const string DB_FILES = "*.db";

		public TDBManagement()
		{
			_dataCacheHashTable = new Hashtable();
			_dbPathDict = new Dictionary<string, string>();
            _lock = new object();

            var baseDir = Application.streamingAssetsPath;

            _databasePath = Path.Combine(baseDir, "GameCore", DB_PATH);
		}

        protected override void Awake()
        {
            base.Awake();

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
                _dbPathDict.Add(tableName, filePath);

                totalTable++;
            }
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
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    using var connection = new SQLiteConnection(_dbPathDict[typeof(T).Name]);
                    var datas = connection.Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }

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
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    using var connection = new SQLiteConnection(_dbPathDict[typeof(T).Name]);
                    var datas = connection.Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }

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
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    using var connection = new SQLiteConnection(_dbPathDict[typeof(T).Name]);
                    var datas = connection.Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }

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
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    using var connection = new SQLiteConnection(_dbPathDict[typeof(T).Name]);
                    var datas = connection.Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }

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
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    using var connection = new SQLiteConnection(_dbPathDict[typeof(T).Name]);
                    var datas = connection.Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }

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
            lock (_lock)
            {
                // double check.
                if (false == _dataCacheHashTable.ContainsKey(typeof(T).Name))
                {
                    using var connection = new SQLiteConnection(_dbPathDict[typeof(T).Name]);
                    var datas = connection.Query<T>($"SELECT * FROM {typeof(T).Name}");

                    _dataCacheHashTable.Add(typeof(T).Name, new TableCache<T>(datas));
                }
            }

            return (_dataCacheHashTable[typeof(T).Name] as TableCache<T>).Count();
        }
    }
}
