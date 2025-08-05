
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDesignerServer.Core.DataManager.ClassData
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.DataManager.TypeData;
    using DataDesignerServer.Core.Services.Interfaces;
    using DataDesignerServer.Core.Types;

    /// <summary>
    /// ClassDataManager.
    /// </summary>
    public sealed partial class ClassDataManager
    {
        private string _srcFolderPath;

        private readonly ILogger<ClassDataManager> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        private readonly TypeDataManager _typeDataManager;

        /// <summary>
        /// GenerateClassGroup dictionary.
        /// key: class name.
        /// value: GenerateClassGroup.
        /// </summary>
        private ConcurrentDictionary<string, GenerateClassGroup> _classGroupDict;

        /// <summary>
        /// Primitive type dictionary.
        /// key: type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _primitiveTypeDict;

        public ClassDataManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassDataManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _typeDataManager = serviceProvider.GetRequiredService<TypeDataManager>();

            _classGroupDict = new ConcurrentDictionary<string, GenerateClassGroup>();
            _primitiveTypeDict = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public bool Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, "Data");

            // Create directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Set up primitive types.
            _primitiveTypeDict = Initialize_PrimitiveTypes();

            // Load class from file.
            if (false == LoadClassDatas(_srcFolderPath))
            {
                _logger.LogError($"Load class failed. path: {_srcFolderPath}");

                return false;
            }

            _logger.LogInformation($"ClassDataManager initialized.");

            return true;
        }

        /// <summary>
        /// Get primitive type or enum from name.
        /// </summary>
        private Type FindTypeOrDefault(string typeName)
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
        /// Get all type.
        /// </summary>
        public IEnumerable<Type> GetClassTypes(Type_ClassData classDataType)
        {
            return _classGroupDict.Values.Where(d => d.ClassDataType.Equals(classDataType)).Select(d => d.Builder?.CreateType());
        }

        /// <summary>
        /// Get all type.
        /// </summary>
        public Type_ClassData GetClassDataType(string className)
        {
            if (false == _classGroupDict.ContainsKey(className))
            {
                return Type_ClassData.None;
            }

            return _classGroupDict[className].ClassDataType;
        }

        /// <summary>
        /// Get all type.
        /// </summary>
        public IEnumerable<Type> GetAllClassType()
        {
            return _classGroupDict.Values.Select(d => d.Builder?.CreateType());
        }

        /// <summary>
        /// Get all row.
        /// </summary>
        public IEnumerable<object> FindRowsOrDefault(string className)
        {
            if (false == _classGroupDict.ContainsKey(className))
            {
                return default;
            }

            return _classGroupDict[className].Rows;
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        public ClassSchema FindSchemaOrDefault(string className)
        {
            if (false == _classGroupDict.ContainsKey(className))
            {
                return default;
            }

            return _classGroupDict[className].Schema;
        }

        /// <summary>
        /// Get table builder.
        /// </summary>
        private TypeBuilder FindTableBuilder(string className)
        {
            // check key.
            if (false == _classGroupDict.ContainsKey(className))
            {
                return default;
            }

            return _classGroupDict[className].Builder;
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        private bool LoadClassDatas(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading class datas.");

            _classGroupDict.Clear();

            foreach (var classDataType in Enum.GetValues<Type_ClassData>())
            {
                string[] dataFiles;
                string[] schemaFiles;
                
                switch(classDataType)
                {
                    case Type_ClassData.Configuration:
                        {
                            var folder = Path.Combine(basePath, FilePath.CONFIG_FOLDER);

                            if (false == Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }

                            dataFiles = Directory.GetFiles(folder, "*.conf", SearchOption.AllDirectories);
                            schemaFiles = Directory.GetFiles(folder, "*.schema", SearchOption.AllDirectories);
                        }
                        break;
                    case Type_ClassData.Table:
                        {
                            var folder = Path.Combine(basePath, FilePath.TABLE_FOLDER);

                            if (false == Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }

                            dataFiles = Directory.GetFiles(folder, "*.tb", SearchOption.AllDirectories);
                            schemaFiles = Directory.GetFiles(folder, "*.schema", SearchOption.AllDirectories);
                        }
                        break;
                    default:
                        continue;
                }

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
                    var type = tb.CreateType();

                    // Create List type.
                    var classListType = typeof(List<>).MakeGenericType(type);
                    var classList = (IList)Activator.CreateInstance(classListType);

                    // Get rows.
                    var rowText = File.ReadAllText(file);
                    var rowDatas = _jsonSerializer.Deserialize(rowText, classList.GetType()) as IList;

                    // Add GenerateClassGroup.
                    var classGroup = new GenerateClassGroup
                    {
                        ClassDataType = classDataType,
                        Rows = rowDatas.OfType<object>().ToList(),
                        Builder = tb,
                        Schema = schema,
                    };

                    AddOrUpdateClassGroup(classGroup);

                    _logger.LogDebug($"Load class: {className}");
                }
            }

            return true;
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private GenerateClassGroup GetClassGroup(string className)
        {
            if (false == _classGroupDict.TryGetValue(className, out var classGroup))
            {
                return null;
            }

            return classGroup;
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private void AddOrUpdateClassGroup(GenerateClassGroup classGroup)
        {
            var typeName = classGroup.Builder.Name;

            _classGroupDict.AddOrUpdate(typeName, classGroup, (key, old) => classGroup);
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private void AddOrUpdateClassGroup(string className, Type_ClassData classDataType = Type_ClassData.None, List<object> rows = default, TypeBuilder tb = default, ClassSchema schema = default)
        {
            // Check type group.
            if (false == _classGroupDict.ContainsKey(className))
            {
                var newClassGroup = new GenerateClassGroup
                {
                    ClassDataType = classDataType,
                };

                _classGroupDict.AddOrUpdate(className, newClassGroup, (key, old) => newClassGroup);
            }

            // Update if data is not default.
            var classGroup = _classGroupDict[className];

            if (default != rows)
            {
                classGroup.Rows = rows;
            }

            if (default != tb)
            {
                classGroup.Builder = tb;
            }

            if (default != schema)
            {
                classGroup.Schema = schema;
            }

            _classGroupDict.AddOrUpdate(className, classGroup, (key, old) => classGroup);
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private bool RemoveClassGroup(string className)
        {
            if (false == _classGroupDict.ContainsKey(className))
            {
                return false;
            }

            var findClassGroup = _classGroupDict[className];

            return _classGroupDict.Remove(className, out findClassGroup);
        }

        /// <summary>
        /// Get module builder.
        /// </summary>
        private ModuleBuilder CreateModuleBuilder()
        {
            var abName = new AssemblyName(typeof(ClassDataManager).Name);
            var ab = AssemblyBuilder
                .DefineDynamicAssembly(abName, AssemblyBuilderAccess.Run);

            return ab.DefineDynamicModule(abName.Name);
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
                var type = FindTypeOrDefault(column.Type);

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
        /// Initialize primitive types.
        /// </summary>
        private Dictionary<string, Type> Initialize_PrimitiveTypes()
        {
            var typeDict = new Dictionary<string, Type>();

            // int.
            typeDict.Add(typeof(int).Name, typeof(int));
            typeDict.Add("int", typeof(int));

            // long.
            typeDict.Add(typeof(long).Name, typeof(int));
            typeDict.Add("long", typeof(long));

            // float.
            typeDict.Add(typeof(float).Name, typeof(int));
            typeDict.Add("float", typeof(float));

            // double.
            typeDict.Add(typeof(double).Name, typeof(int));
            typeDict.Add("double", typeof(double));

            // string.
            typeDict.Add(typeof(string).Name, typeof(int));
            typeDict.Add("string", typeof(string));

            // bool.
            typeDict.Add(typeof(bool).Name, typeof(int));
            typeDict.Add("bool", typeof(bool));

            // DateTime.
            typeDict.Add(typeof(DateTime).Name, typeof(int));
            typeDict.Add("date", typeof(DateTime));

            return typeDict;
        }

        /// <summary>
        /// Write data file.
        /// </summary>
        private bool FlushFiles(string className)
        {
            var classGroup = GetClassGroup(className);

            if (classGroup == null)
            {
                _logger.LogError($"failed to flush files.");
                return false;
            }

            // Check data file.
            var dataFile = Path.Combine(_srcFolderPath, $"{className}.conf");

            // Check data file.
            var schemaFile = Path.Combine(_srcFolderPath, $"{className}.schema");

            switch (classGroup.ClassDataType)
            {
                case Type_ClassData.Configuration:
                    dataFile = Path.Combine(_srcFolderPath, FilePath.CONFIG_FOLDER, $"{className}.conf");
                    schemaFile = Path.Combine(_srcFolderPath, FilePath.CONFIG_FOLDER, $"{className}.schema");
                    break;
                case Type_ClassData.Table:
                    dataFile = Path.Combine(_srcFolderPath, FilePath.TABLE_FOLDER, $"{className}.tb");
                    schemaFile = Path.Combine(_srcFolderPath, FilePath.TABLE_FOLDER, $"{className}.schema");
                    break;
                default:
                    return false;
            }

            // File check existance.
            if (false == File.Exists(dataFile))
            {
                File.Create(dataFile).Dispose();
            }

            if (false == File.Exists(schemaFile))
            {
                File.Create(schemaFile).Dispose();
            }

            // Get rows.
            var rows = FindRowsOrDefault(className);
            if (default == rows)
            {
                return false;
            }

            // Get schema.
            var schema = FindSchemaOrDefault(className);
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

        /// <summary>
        /// Delete files.
        /// </summary>
        private bool RemoveFiles(string className)
        {
            var classGroup = GetClassGroup(className);

            if (classGroup == null)
            {
                _logger.LogError($"failed to remove files.");
                return false;
            }

            switch (classGroup.ClassDataType)
            {
                case Type_ClassData.Configuration:
                    File.Delete(Path.Combine(_srcFolderPath, FilePath.CONFIG_FOLDER, $"{className}.conf"));
                    File.Delete(Path.Combine(_srcFolderPath, FilePath.CONFIG_FOLDER, $"{className}.schema"));
                    break;
                case Type_ClassData.Table:
                    File.Delete(Path.Combine(_srcFolderPath, FilePath.TABLE_FOLDER, $"{className}.tb"));
                    File.Delete(Path.Combine(_srcFolderPath, FilePath.TABLE_FOLDER, $"{className}.schema"));
                    break;
            }

            return true;
        }
    }
}
