using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDesigner.Core.TypeManager
{
    using DataDesigner.Core.Data;
    using DataDesigner.Core.Services.Interfaces;

    /// <summary>
    /// ClassManager.
    /// Desc: Manage class types.
    /// </summary>
    public sealed partial class ClassManager
    {
        private const string ALL_DATA_FILE = "*.obj";
        private const string ALL_SCHEMA_FILE = "*.schema";
        private const string SRC_FOLDER = $"DataDesigner/{FilePath.CLASS_FOLDER}";

        private readonly ILogger<ClassManager> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        private readonly EnumManager _enumManager;

        private string _srcFolderPath;
        private ModuleBuilder _mb;

        /// <summary>
        /// Data dictionary.
        /// key: class name.
        /// value: row datas of class.
        /// </summary>
        private ConcurrentDictionary<string, List<object>> _classRowDict;

        /// <summary>
        /// key: type name.
        /// value: TypeBuilder.
        /// </summary>
        private ConcurrentDictionary<string, TypeBuilder> _classTBDict;

        /// <summary>
        /// key: type name.
        /// value: schema string.
        /// </summary>
        private ConcurrentDictionary<string, ClassSchema> _classSchemaDict;

        /// <summary>
        /// key: primitive type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _primitiveTypeDict;

        public ClassManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _enumManager = serviceProvider.GetRequiredService<EnumManager>();

            _classRowDict = new ConcurrentDictionary<string, List<object>>();
            _classTBDict = new ConcurrentDictionary<string, TypeBuilder>();
            _classSchemaDict = new ConcurrentDictionary<string, ClassSchema>();

            _primitiveTypeDict = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public bool Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, SRC_FOLDER);

            // Create directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Set up primitive types.
            _primitiveTypeDict.Clear();
            _primitiveTypeDict.Add("int", typeof(int));
            _primitiveTypeDict.Add("long", typeof(long));
            _primitiveTypeDict.Add("float", typeof(float));
            _primitiveTypeDict.Add("double", typeof(double));
            _primitiveTypeDict.Add("string", typeof(string));
            _primitiveTypeDict.Add("bool", typeof(bool));
            _primitiveTypeDict.Add("date", typeof(DateTime));

            // Create module builder.
            var abName = new AssemblyName(typeof(ClassManager).Name);
            var ab = AssemblyBuilder
                .DefineDynamicAssembly(abName, AssemblyBuilderAccess.Run);

            // Dynamic module.
            _mb = ab.DefineDynamicModule(abName.Name);

            // Load class.
            if (false == LoadClasses(_srcFolderPath))
            {
                _logger.LogError("Load class failed.");

                return false;
            }

            _logger.LogInformation($"ClassManager initialized.");

            return true;
        }

        #region PRIVATE

        /// <summary>
        /// Load datas.
        /// </summary>
        private bool LoadClasses(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading classes.");

            var dataFiles = Directory.GetFiles(basePath, ALL_DATA_FILE, SearchOption.AllDirectories);
            var schemaFiles = Directory.GetFiles(basePath, ALL_SCHEMA_FILE, SearchOption.AllDirectories);

            _classRowDict.Clear();

            foreach (var file in dataFiles)
            {
                var className = Path.GetFileNameWithoutExtension(file);

                // Check schema file.
                var schemaFile = schemaFiles.FirstOrDefault(d => Path.GetFileNameWithoutExtension(d).Equals(className));
                if (string.IsNullOrEmpty(schemaFile))
                {
                    return false;
                }

                // Read files.
                var schemaText = File.ReadAllText(schemaFile);
                var schema = _jsonSerializer.Deserialize<ClassSchema>(schemaText);
                
                // Create TypeBuilder.
                var tb = CreateTypeBuilder(className, schema);
                if (null == tb)
                {
                    return false;
                }

                // Update TypeBuilder.
                var classType = tb.CreateType();
                SetTypeBuilder(classType.Name, tb);

                // Get row datas.
                var rowText = File.ReadAllText(file);
                var rowDatas = _jsonSerializer.Deserialize<List<object>>(rowText);

                // Add row datas.
                SetRows(classType.Name, rowDatas);

                // Add schema.
                if (false == _classSchemaDict.TryAdd(classType.Name, schema))
                {
                    return false;
                }

                _logger.LogDebug($"Load class: {className}");
            }

            return true;
        }

        /// <summary>
        /// Get type from name.
        /// </summary>
        private TypeBuilder CreateTypeBuilder(string className, ClassSchema schema)
        {
            // Define type.
            var tb = _mb.DefineType(className, TypeAttributes.Public);

            foreach (var column in schema.Columns)
            {
                // Get Type.
                var type = GetTypeFromName(column.Name);

                if (null == type)
                {
                    _logger.LogError($"Update schema failed. class name: {className}, column type: {column.Name}");

                    return null;
                }

                // Define property.
                tb.DefineProperty(column.Name, PropertyAttributes.HasDefault, type, null);
            }

            return tb;
        }

        /// <summary>
        /// Add or update TypeBuidler.
        /// </summary>
        private void SetTypeBuilder(string typeName, TypeBuilder tb)
        {
            if (false == _classTBDict.ContainsKey(typeName))
            {
                _classTBDict.TryAdd(typeName, null);
            }

            _classTBDict.AddOrUpdate(typeName, tb, (key, old) => tb);
        }

        /// <summary>
        /// Get TypeBuilder.
        /// </summary>
        private TypeBuilder GetTypeBuilder(string typeName)
        {
            return _classTBDict.GetValueOrDefault(typeName);
        }
        
        /// <summary>
        /// Get primitive type or enum from name.
        /// </summary>
        private Type GetTypeFromName(string typeName)
        {
            // Find from primitive types.
            if (_primitiveTypeDict.ContainsKey(typeName))
            {
                return _primitiveTypeDict[typeName];
            }

            // Find from enum types.
            return _enumManager.GetTypes().FirstOrDefault(d => d.Name.Equals(typeName));
        }

        /// <summary>
        /// Get rows for type.
        /// </summary>
        private List<object> GetRows(string typeName)
        {
            return _classRowDict.GetValueOrDefault(typeName);
        }

        /// <summary>
        /// Set rows for type.
        /// </summary>
        private void SetRows(string typeName, List<object> list)
        {
            if (false == _classRowDict.ContainsKey(typeName))
            {
                _classRowDict.TryAdd(typeName, null);
            }

            _classRowDict.AddOrUpdate(typeName, list, (key, old) => list);
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        private ClassSchema GetSchema(string typeName)
        {
            return _classSchemaDict.GetValueOrDefault(typeName);
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        private ClassSchema SetSchema(string typeName, ClassSchema schema)
        {
            return _classSchemaDict.AddOrUpdate(typeName, schema, (key, old) => schema);
        }

        /// <summary>
        /// Write data file.
        /// </summary>
        private bool FlushFiles(string typeName)
        {
            // Check data file.
            var dataFile = Path.Combine(_srcFolderPath, $"{typeName}.obj");
            if (false == File.Exists(dataFile))
            {
                return false;
            }

            // Check data file.
            var schemaFile = Path.Combine(_srcFolderPath, $"{typeName}.schema");
            if (false == File.Exists(schemaFile))
            {
                return false;
            }

            // Get rows.
            var rows = GetRows(typeName);
            if (default == rows)
            {
                return false;
            }

            // Get schema.
            var schema = GetSchema(typeName);
            if (default == schema)
            {
                return false;
            }

            // Write data file.
            var rowsJson = _jsonSerializer.Serialize(rows);
            File.WriteAllText(dataFile, rowsJson);

            // Write file.
            var schemaJson = _jsonSerializer.Serialize(schema);
            File.WriteAllText(schemaFile, schemaJson);

            return true;
        }

        #endregion 
    }
}
