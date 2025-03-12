//using Microsoft.Extensions.Logging;
//using System.Collections;
//using System.Reflection;
//using System.Runtime.Loader;

//namespace GameCore
//{
//	public sealed class TDBService 
//	{
//		private readonly ILogger<TDBService> _logger;
//		private readonly Dictionary<string, SqliteConnection> _databaseConnection;
//		private readonly Hashtable _dataCacheHashTable;
//		private readonly Dictionary<Type, ITDBConfigTable> _configTables;

//		private readonly string _rootPath;
//		private readonly string _databasePath;

//		public const string DATABASE_PATH = "Data";
//		public const string CONFIG_FILENAME = "config.db";
//		public const string ASSEMBLY_DLL = "GameCore.dll";
//		public const string ASSEMBLY_NAME = "GameCore";

//		public TDBService(ILogger<TDBService> logger)
//		{
//			_logger = logger;

//			_dataCacheHashTable = new Hashtable();
//			_databaseConnection = new Dictionary<string, SqliteConnection>();
//			_configTables = new Dictionary<Type, ITDBConfigTable>();

//			_rootPath = Path.GetDirectoryName(System.AppContext.BaseDirectory);
//			_databasePath = Path.Combine(_rootPath, DATABASE_PATH);
//		}

//		public bool Initialize()
//		{
//			var filePaths = Directory.GetFiles(_databasePath, "*.db", SearchOption.AllDirectories);

//			Assembly asm = null;
//			if (true == File.Exists(Path.Combine(_rootPath, ASSEMBLY_DLL)))
//			{
//				asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(_rootPath, ASSEMBLY_DLL));
//			}
//			else
//			{
//				asm = Assembly.GetExecutingAssembly();
//			}

//			int tableTotal = 0;

//			if (filePaths == null)
//			{
//				_logger.LogInformation($"::: Table Prepare Complete, Total: {tableTotal} ");
//				return false;
//			}

//			foreach (var filePath in filePaths)
//			{
//				if (true == filePath.Contains(CONFIG_FILENAME))
//				{
//					LoadConfigTable(filePath);
//					continue;
//				}

//				var connectionString = $"Data Source={filePath};";
//				var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
//				SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

//				var tableName = Path.GetFileNameWithoutExtension(filePath);

//				var tableType = asm.GetType($"{ASSEMBLY_NAME}.{tableName}");

//				_databaseConnection.Add(tableName, connection);

//				try
//				{
//					var method = this.GetType().GetMethod("CreateDataCacheTable");
//					var generic = method.MakeGenericMethod(tableType);
//					generic.Invoke(this, null);
//				}
//				catch (Exception ex)
//				{
//					_logger.LogInformation($"table:{tableName}, type:{tableType}, {ex.Message}");
//				}

//				tableTotal++;
//			}

//			_logger.LogInformation($"::: Table Prepare Complete, Total: {tableTotal}");
//			_logger.LogInformation($"::: Config Prepare Complete, Total: {_configTables.Count}");
//			return true;
//		}

//		private void LoadConfigTable(string filePath)
//		{
//			var connectionString = $"Data Source={filePath};";
//			var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
//			SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

//			var tableName = "ConfigTableBase";
//			var configDatas = connection.Query<ConfigTableBase>($"SELECT * FROM {tableName}");

//			Assembly asm = null;
//			if (true == File.Exists(Path.Combine(_rootPath, ASSEMBLY_DLL)))
//			{
//				asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(_rootPath, ASSEMBLY_DLL));
//			}
//			else
//			{
//				asm = Assembly.GetExecutingAssembly();
//			}

//			foreach (var configData in configDatas)
//			{
//				var classType = asm.GetType($"{ASSEMBLY_NAME}.{configData.TableName}");

//				try
//				{
//					var method = this.GetType().GetMethod("CreateConfigTable");
//					var generic = method.MakeGenericMethod(classType);
//					generic.Invoke(this, new object[] { configData.JsonData });
//				}
//				catch (Exception ex)
//				{
//					_logger.LogError($"config table:{tableName}, type:{classType}, {ex.Message}");
//				}
//			}
//		}

//		public void CreateConfigTable<T>(string json) where T : class, ITDBConfigTable
//		{
//			var config = JsonConvert.DeserializeObject<T>(json);

//			if (true == _configTables.ContainsKey(typeof(T)))
//			{
//				_logger.LogError($"config table:{typeof(T)}, already added.");
//				return;
//			}

//			_configTables.Add(typeof(T), config);
//		}

//		public T GetConfig<T>() where T : class, ITDBConfigTable
//		{
//			if (false == _configTables.ContainsKey(typeof(T)))
//			{
//				_logger.LogError($"config table:{typeof(T)}, not exist config table.");
//				return null;
//			}

//			return _configTables[typeof(T)] as T;
//		}

//		/// <summary>
//		/// create DataCacheTable
//		/// </summary>
//		public DataCacheTable<T> CreateDataCacheTable<T>() where T : class, ITDBDataTable, new()
//		{
//			var allList = SelectAllFromSQL<T>();
//			var dataCache = new DataCacheTable<T>(allList);
//			if (true == _dataCacheHashTable.ContainsKey(typeof(T).Name))
//			{
//				return _dataCacheHashTable[typeof(T).Name] as DataCacheTable<T>;
//			}

//			_dataCacheHashTable.Add(typeof(T).Name, dataCache);

//			//Console.WriteLine($"TableCreate : {typeof(T).Name} - {dataCache.Count()}");

//			return dataCache;
//		}

//		/// <summary>
//		/// Retreive DataCacheTable
//		/// </summary>
//		private DataCacheTable<T> GetDataCacheTable<T>() where T : class, ITDBDataTable, new()
//		{
//			if (true == _dataCacheHashTable.ContainsKey(typeof(T).Name))
//			{
//				return _dataCacheHashTable[typeof(T).Name] as DataCacheTable<T>;
//			}

//			return CreateDataCacheTable<T>();
//		}

//		/// <summary>
//		/// select all data
//		/// </summary>
//		public List<T> SelectAllFromSQL<T>() where T : class, new()
//		{
//			if (_databaseConnection.ContainsKey(typeof(T).Name))
//			{
//				var conn = _databaseConnection[typeof(T).Name];
//				if (null == conn)
//				{
//					return null;
//				}

//				return conn.Query<T>($"SELECT * FROM {typeof(T).Name}").ToList();
//			}

//			return null;
//		}


//		/// <summary>
//		/// select all data
//		/// </summary>
//		public List<T> SelectAllFromSQL<T>(string tableName) where T : class, new()
//		{
//			if (_databaseConnection.ContainsKey(tableName))
//			{
//				var conn = _databaseConnection[tableName];
//				if (null == conn)
//				{
//					return null;
//				}

//				return conn.Query<T>($"SELECT * FROM {tableName}").ToList();
//			}

//			return null;
//		}

//		/// <summary>
//		/// select all data
//		/// </summary>
//		public List<T> SelectFromSQL<T>(string tableName, string statement) where T : class, new()
//		{
//			if (_databaseConnection.ContainsKey(tableName))
//			{
//				var conn = _databaseConnection[tableName];
//				if (null == conn)
//				{
//					return null;
//				}

//				return conn.Query<T>($"SELECT * FROM {tableName} {statement}").ToList();
//			}

//			return null;
//		}

//		/// <summary>
//		/// select all data
//		/// </summary>
//		public IEnumerable<T> SelectAll<T>() where T : class, ITDBDataTable, new()
//		{
//			return GetDataCacheTable<T>().SelectAll();
//		}

//		public T GetById<T>(int id) where T : class, ITDBDataTable, new()
//		{
//			var tb = GetDataCacheTable<T>();
//			if (null == tb)
//			{
//				Console.WriteLine($"tb is null - {typeof(T).Name}, {id}");
//				return null;
//			}

//			return tb.GetById(id);
//		}

//		/// <summary>
//		/// retreive first data from predicate
//		/// </summary>
//		public T First<T>(Func<T, bool> predExpr) where T : class, ITDBDataTable, new()
//		{
//			return GetDataCacheTable<T>()?.First(predExpr);
//		}

//		/// <summary>
//		/// retreive first data from predicate
//		/// </summary>
//		public T Last<T>(Func<T, bool> predExpr) where T : class, ITDBDataTable, new()
//		{
//			return GetDataCacheTable<T>()?.Last(predExpr);
//		}

//		/// <summary>
//		/// select data from predicate
//		/// </summary>
//		public IEnumerable<T> Select<T>(Func<T, bool> predExpr) where T : class, ITDBDataTable, new()
//		{
//			return GetDataCacheTable<T>()?.Select(predExpr);
//		}

//	}
//}
