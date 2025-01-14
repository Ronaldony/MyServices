using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDesignerServer.Core.TypeManager.Enum
{
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// EnumLoader.
    /// Description: Load enum datas from .type format files.
    /// </summary>
    public sealed partial class EnumManager
    {
        private const string ALL_TYPE_FILES = "*.type";
        private const string ALL_SCHEMA_FILE = "*.schema";
        
        private string _srcFolderPath;

        private readonly ILogger<EnumManager> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        /// <summary>
        /// Type dictionary.
        /// key : type name.
        /// value: type.
        /// </summary>
        private readonly Dictionary<string, Type> _typeDict;

        /// <summary>
        /// Enum value dict.
        /// key: enum name.
        /// value: enum datas.
        /// </summary>
        private ConcurrentDictionary<string, List<EnumSchemaRow>> _enumRowDict;

        /// <summary>
        /// key: type name.
        /// value: EnumBuilder.
        /// </summary>
        private ConcurrentDictionary<string, EnumBuilder> _ebDict;

        /// <summary>
        /// key: type name.
        /// value: schema string.
        /// </summary>
        private ConcurrentDictionary<string, EnumSchema> _schemaDict;

        public EnumManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _typeDict = new Dictionary<string, Type>();
            _enumRowDict = new ConcurrentDictionary<string, List<EnumSchemaRow>>();
            _ebDict = new ConcurrentDictionary<string, EnumBuilder>();
            _schemaDict = new ConcurrentDictionary<string, EnumSchema>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public bool Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, "Data", FilePath.CLASS_FOLDER);

            // Create directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Load enum from file.
            if (false == LoadEnums(_srcFolderPath))
            {
                _logger.LogError($"Load enum failed. path: {_srcFolderPath}");
                return false;
            }

            _logger.LogInformation($"EnumManager initialized.");

            return true;
        }

        /// <summary>
        /// Get types.
        /// </summary>
        /// <returns></returns>
        public Type FindTypeOrDefault(string typeName)
        {
            return _typeDict.GetValueOrDefault(typeName);
        }

        /// <summary>
        /// Get schema names.
        /// </summary>
        public IEnumerable<string> GetSchemaNames()
        {
            return _schemaDict.Keys;
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        public EnumSchema FindSchemaOrDefault(string schemaName)
        {
            return _schemaDict.GetValueOrDefault(schemaName);
        }

        /// <summary>
        /// Get rows for type.
        /// </summary>
        public List<EnumSchemaRow> GetRowsOrDefault(string typeName)
        {
            return _enumRowDict.GetValueOrDefault(typeName);
        }

        #region PRIVATE

        /// <summary>
        /// Load enums.
        /// </summary>
        private bool LoadEnums(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading enum.");

            var enumFiles = Directory.GetFiles(basePath, ALL_TYPE_FILES, SearchOption.AllDirectories);
            var schemaFiles = Directory.GetFiles(basePath, ALL_SCHEMA_FILE, SearchOption.AllDirectories);

            _enumRowDict.Clear();

            foreach (var file in enumFiles)
            {
                var enumName = Path.GetFileNameWithoutExtension(file);

                // Schema file.
                var schemaFile = schemaFiles.FirstOrDefault(d => Path.GetFileNameWithoutExtension(d).Equals(enumName));
                if (string.IsNullOrEmpty(schemaFile))
                {
                    return false;
                }

                // Read rows.
                var rowText = File.ReadAllText(file);
                var rowDatas = _jsonSerializer.Deserialize<List<EnumSchemaRow>>(rowText);

                // Create TypeBuilder.
                var eb = CreateEnumBuilder(enumName, rowDatas);
                if (null == eb)
                {
                    return false;
                }

                // Update TypeBuilder.
                var enumType = eb.CreateType();
                SetEnumBuilder(enumName, eb);

                // Add row datas.
                SetRows(enumType.Name, rowDatas);

                // Read schema.
                var schemaText = File.ReadAllText(schemaFile);
                var schema = _jsonSerializer.Deserialize<EnumSchema>(schemaText);

                // Add schema.
                if (false == _schemaDict.TryAdd(enumType.Name, schema))
                {
                    return false;
                }

                _logger.LogDebug($"Load enum: {enumName}");
            }

            return true;
        }

        /// <summary>
        /// Create type buidler.
        /// </summary>
        private EnumBuilder CreateEnumBuilder(string enumName, List<EnumSchemaRow> rows)
        {
            // Define type.
            var mb = CreateModuleBuilder();
            var eb = mb.DefineEnum(enumName, TypeAttributes.Public, typeof(int));

            // Id.
            foreach (var row in rows)
            {
                eb.DefineLiteral(row.Name, row.Id);
            }

            return eb;
        }

        /// <summary>
        /// Add or update EnumBuidler.
        /// </summary>
        private void SetEnumBuilder(string enumName, EnumBuilder eb)
        {
            if (false == _ebDict.ContainsKey(enumName))
            {
                _ebDict.TryAdd(enumName, null);
            }

            _ebDict.AddOrUpdate(enumName, eb, (key, old) => eb);

            // Update type dictionary.
            if (_typeDict.ContainsKey(enumName))
            {
                _typeDict[enumName] = eb.CreateType();
            }
            else
            {
                _typeDict.Add(enumName, eb.CreateType());
            }
        }

        /// <summary>
        /// Set rows for type.
        /// </summary>
        private void SetRows(string typeName, List<EnumSchemaRow> list)
        {
            if (false == _enumRowDict.ContainsKey(typeName))
            {
                _enumRowDict.TryAdd(typeName, null);
            }

            _enumRowDict.AddOrUpdate(typeName, list, (key, old) => list);
        }

        /// <summary>
        /// Get module builder.
        /// </summary>
        private ModuleBuilder CreateModuleBuilder()
        {
            var abName = new AssemblyName(typeof(EnumManager).Name);
            var ab = AssemblyBuilder
                .DefineDynamicAssembly(abName, AssemblyBuilderAccess.Run);

            return ab.DefineDynamicModule(abName.Name);
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        private EnumSchema SetSchema(string typeName, EnumSchema schema)
        {
            return _schemaDict.AddOrUpdate(typeName, schema, (key, old) => schema);
        }

        /// <summary>
        /// Get primitive type or enum from name.
        /// </summary>
        private Type GetEnumType(string enumName)
        {
            if (false == _ebDict.ContainsKey(enumName))
            {
                return null;
            }

            return _ebDict[enumName].CreateType();
        }

        /// <summary>
        /// Write data file.
        /// </summary>
        private bool FlushFiles(string typeName)
        {
            // Check data file.
            var enumFile = Path.Combine(_srcFolderPath, $"{typeName}.type");
            if (false == File.Exists(enumFile))
            {
                File.Create(enumFile).Dispose();
            }

            // Check data file.
            var schemaFile = Path.Combine(_srcFolderPath, $"{typeName}.schema");
            if (false == File.Exists(schemaFile))
            {
                File.Create(schemaFile).Dispose();
            }

            // Get rows.
            var rows = GetRowsOrDefault(typeName);
            if (default == rows)
            {
                return false;
            }

            // Get schema.
            var schema = FindSchemaOrDefault(typeName);
            if (default == schema)
            {
                return false;
            }

            // Write data file.
            var rowsJson = _jsonSerializer.Serialize(rows);
            File.WriteAllText(enumFile, rowsJson);

            // Write file.
            var schemaJson = _jsonSerializer.Serialize(schema);
            File.WriteAllText(schemaFile, schemaJson);

            return true;
        }

        #endregion
    }
}
