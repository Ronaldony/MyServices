using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDesignerServer.Core.DataManager.ConfigData
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// TableManager.
    /// Desc: Manage table types.
    /// </summary>
    public sealed partial class ConfigDataManager
    {
        private const string ALL_DATA_FILE = "*.conf";
        private const string ALL_SCHEMA_FILE = "*.schema";

        private string _srcFolderPath;

        private readonly ILogger<ConfigDataManager> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        private readonly TypeDataManager _typeDataManager;

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
        private ConcurrentDictionary<string, TypeBuilder> _tbDict;

        /// <summary>
        /// key: type name.
        /// value: schema string.
        /// </summary>
        private ConcurrentDictionary<string, ClassSchema> _schemaDict;

        /// <summary>
        /// key: primitive type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _primitiveTypeDict;

        public ConfigDataManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ConfigDataManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _typeDataManager = serviceProvider.GetRequiredService<TypeDataManager>();

            _classRowDict = new ConcurrentDictionary<string, List<object>>();
            _tbDict = new ConcurrentDictionary<string, TypeBuilder>();
            _schemaDict = new ConcurrentDictionary<string, ClassSchema>();

            _primitiveTypeDict = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public bool Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, "Data", FilePath.CONFIG_FOLDER);

            // Create directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Set up primitive types.
            _primitiveTypeDict = Initialize_PrimitiveTypes();

            // Load class from file.
            if (false == LoadTables(_srcFolderPath))
            {
                _logger.LogError($"Load class failed. path: {_srcFolderPath}");

                return false;
            }

            _logger.LogInformation($"ClassManager initialized.");

            return true;
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        public ClassSchema FindSchemaOrDefault(string schemaName)
        {
            return _schemaDict.GetValueOrDefault(schemaName);
        }

        /// <summary>
        /// Get all schema names.
        /// </summary>
        public IEnumerable<string> GetSchemaNames()
        {
            return _schemaDict.Keys;
        }

        /// <summary>
        /// Get all row.
        /// </summary>
        public IEnumerable<object> FindRowsOrDefault(string schemaName)
        {
            return _classRowDict.GetValueOrDefault(schemaName);
        }

        /// <summary>
        /// Initialize primitive types.
        /// </summary>
        private Dictionary<string, Type> Initialize_PrimitiveTypes()
        {
            var typeDict = new Dictionary<string, Type>();

            typeDict.Add(typeof(int).Name, typeof(int));
            typeDict.Add("int", typeof(int));
            typeDict.Add(typeof(long).Name, typeof(int));
            typeDict.Add("long", typeof(long));
            typeDict.Add(typeof(float).Name, typeof(int));
            typeDict.Add("float", typeof(float));
            typeDict.Add(typeof(double).Name, typeof(int));
            typeDict.Add("double", typeof(double));
            typeDict.Add(typeof(string).Name, typeof(int));
            typeDict.Add("string", typeof(string));
            typeDict.Add(typeof(bool).Name, typeof(int));
            typeDict.Add("bool", typeof(bool));
            typeDict.Add(typeof(DateTime).Name, typeof(int));
            typeDict.Add("date", typeof(DateTime));

            return typeDict;
        }

        /// <summary>
        /// Get module builder.
        /// </summary>
        private ModuleBuilder CreateModuleBuilder()
        {
            var abName = new AssemblyName(typeof(ConfigDataManager).Name);
            var ab = AssemblyBuilder
                .DefineDynamicAssembly(abName, AssemblyBuilderAccess.Run);

            return ab.DefineDynamicModule(abName.Name);
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        private bool LoadTables(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading tables.");

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

                // Create type list.
                var classListType = typeof(List<>).MakeGenericType(classType);
                var classList = (IList)Activator.CreateInstance(classListType);

                // Get row datas.
                var rowText = File.ReadAllText(file);
                var rowDatas = _jsonSerializer.Deserialize(rowText, classList.GetType()) as IList;

                // Add row datas.
                SetRows(classType.Name, classList.OfType<object>().ToList());

                // Add schema.
                if (false == _schemaDict.TryAdd(classType.Name, schema))
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
            var mb = CreateModuleBuilder();
            var tb = mb.DefineType(className, TypeAttributes.Public);

            foreach (var column in schema.Columns)
            {
                // Get Type.
                var type = GetTypeOrDefault(column.Type);

                if (null == type)
                {
                    _logger.LogError($"Update schema failed. class name: {className}, column type: {column.Name}");

                    return null;
                }

                // Define property, field.
                var propBuilder = tb.DefineProperty(column.Name, PropertyAttributes.HasDefault, type, null);
                var fieldBuilder = tb.DefineField($"_{column.Name}", type, FieldAttributes.Private);

                // Getter.
                var getter = tb.DefineMethod(
                    $"get_{column.Name}",   // name
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,    // attributes.
                    type,               // return type.
                    Type.EmptyTypes);   // parameters.

                var getterIL = getter.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);

                propBuilder.SetGetMethod(getter);

                // Setter.
                var setter = tb.DefineMethod(
                    $"set_{column.Name}",
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null,
                    new[] { type });

                var setterIL = setter.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                propBuilder.SetSetMethod(setter);
            }

            return tb;
        }

        /// <summary>
        /// Add or update TypeBuidler.
        /// </summary>
        private void SetTypeBuilder(string typeName, TypeBuilder tb)
        {
            if (false == _tbDict.ContainsKey(typeName))
            {
                _tbDict.TryAdd(typeName, null);
            }

            _tbDict.AddOrUpdate(typeName, tb, (key, old) => tb);
        }

        /// <summary>
        /// Get TypeBuilder.
        /// </summary>
        private TypeBuilder GetTypeBuilder(string typeName)
        {
            return _tbDict.GetValueOrDefault(typeName);
        }

        /// <summary>
        /// Get primitive type or enum from name.
        /// </summary>
        private Type GetTypeOrDefault(string typeName)
        {
            // Find from primitive types.
            if (_primitiveTypeDict.ContainsKey(typeName))
            {
                return _primitiveTypeDict[typeName];
            }

            // Find from enum types.
            return _typeDataManager.FindTypeOrDefault(typeName);
        }

        /// <summary>
        /// Get primitive type or enum from name.
        /// </summary>
        private Type GetClassType(string className)
        {
            if (false == _tbDict.ContainsKey(className))
            {
                return null;
            }

            return _tbDict[className].CreateType();
        }

        /// <summary>
        /// Get rows for type.
        /// </summary>
        private List<object> GetRowsOrDefault(string typeName)
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
        private ClassSchema SetSchema(string typeName, ClassSchema schema)
        {
            return _schemaDict.AddOrUpdate(typeName, schema, (key, old) => schema);
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
                File.Create(dataFile).Dispose();
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
            File.WriteAllText(dataFile, rowsJson);

            // Write file.
            var schemaJson = _jsonSerializer.Serialize(schema);
            File.WriteAllText(schemaFile, schemaJson);

            return true;
        }
    }
}
