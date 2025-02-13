using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDesignerServer.Core.DataManager.TypeData
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// TypeManager.
    /// </summary>
    public sealed partial class TypeDataManager
    {
        private const string ALL_TYPE_FILES = "*.type";
        private const string ALL_SCHEMA_FILE = "*.schema";

        private string _srcFolderPath;

        private readonly ILogger<TypeDataManager> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        private ModuleBuilder _moduleBuilder;

        private AssemblyBuilder _assemblyBuilder;

        /// <summary>
        /// Default columns of type schema.
        /// </summary>
        private List<TypeSchemaColumn> _typeSchemaColumns;

        /// <summary>
        /// Enum value dict.
        /// key: enum name.
        /// value: enum datas.
        /// </summary>
        private ConcurrentDictionary<string, List<TypeSchemaRow>> _typeRowDict;

        /// <summary>
        /// key: type name.
        /// value: EnumBuilder.
        /// </summary>
        private ConcurrentDictionary<string, EnumBuilder> _ebDict { get; set; }

        /// <summary>
        /// key: type name.
        /// value: schema string.
        /// </summary>
        private ConcurrentDictionary<string, TypeSchema> _schemaDict;

        /// <summary>
        /// key: type name.
        /// value: schema string.
        /// </summary>
        private ConcurrentDictionary<string, TypeSchema> _primitiveSchemaDict;

        public TypeDataManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<TypeDataManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _typeRowDict = new ConcurrentDictionary<string, List<TypeSchemaRow>>();
            _ebDict = new ConcurrentDictionary<string, EnumBuilder>();
            _schemaDict = new ConcurrentDictionary<string, TypeSchema>();
            _primitiveSchemaDict = new ConcurrentDictionary<string, TypeSchema>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public bool Initialize(string basePath)
        {
            // module builder.
            _moduleBuilder = CreateModuleBuilder();

            _srcFolderPath = Path.Combine(basePath, "Data", FilePath.TYPE_FOLDER);

            // Create directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Load enum from file.
            if (false == LoadTypes(_srcFolderPath))
            {
                _logger.LogError($"Load enum failed. path: {_srcFolderPath}");
                return false;
            }

            _typeSchemaColumns = Initialize_SchemaColumns();

            _logger.LogInformation($"EnumManager initialized.");

            return true;
        }

        /// <summary>
        /// Get types.
        /// </summary>
        public Type FindTypeOrDefault(string typeName)
        {
            if (_ebDict.ContainsKey(typeName))
            {
                return _ebDict[typeName].CreateType();
            }

            return default;
        }

        /// <summary>
        /// Get schema names only custom type.
        /// </summary>
        public IEnumerable<Type> GetAllType()
        {
            return _ebDict.Values.Select(d => d.AsType());
        }

        /// <summary>
        /// Get schema names with primitive types(int, float, bool, string etc..).
        /// </summary>
        public IEnumerable<string> GetTypeSchemasWithPrimitive()
        {
            var typeSchemas = new List<string>();

            typeSchemas.AddRange(_primitiveSchemaDict.Keys);
            typeSchemas.AddRange(_schemaDict.Keys);

            return typeSchemas;
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        public TypeSchema FindSchemaOrDefault(string schemaName)
        {
            return _schemaDict.GetValueOrDefault(schemaName);
        }

        /// <summary>
        /// Get rows for type.
        /// </summary>
        public List<TypeSchemaRow> GetRowsOrDefault(string typeName)
        {
            return _typeRowDict.GetValueOrDefault(typeName);
        }

        /// <summary>
        /// Get type schema columns.
        /// </summary>
        /// <returns></returns>
        public List<TypeSchemaColumn> GetSchemaColumns()
        {
            return _typeSchemaColumns;
        }

        /// <summary>
        /// Initialize primitive types.
        /// </summary>
        private Dictionary<string, TypeSchema> Initialize_PrimitiveTypes()
        {
            var typeDict = new Dictionary<string, TypeSchema>();

            typeDict.Add("int", new TypeSchema
            {
                Name = "int",
                Category = "primitive",
                Description = "int"
            });

            typeDict.Add("long", new TypeSchema
            {
                Name = "long",
                Category = "primitive",
                Description = "long"
            });

            typeDict.Add("float", new TypeSchema
            {
                Name = "float",
                Category = "primitive",
                Description = "float"
            });

            typeDict.Add("double", new TypeSchema
            {
                Name = "double",
                Category = "primitive",
                Description = "double"
            });

            typeDict.Add("string", new TypeSchema
            {
                Name = "string",
                Category = "primitive",
                Description = "string"
            });

            typeDict.Add("bool", new TypeSchema
            {
                Name = "bool",
                Category = "primitive",
                Description = "bool"
            });

            typeDict.Add("date", new TypeSchema
            {
                Name = "date",
                Category = "primitive",
                Description = "date"
            });

            return typeDict;
        }

        /// <summary>
        /// Initialize SchemaColumns
        /// </summary>
        /// <returns></returns>
        private List<TypeSchemaColumn> Initialize_SchemaColumns()
        {
            var columns = new List<TypeSchemaColumn>();

            columns.Add(new TypeSchemaColumn
            {
                Name = "Id",
                Type = "int"
            });

            columns.Add(new TypeSchemaColumn
            {
                Name = "Name",
                Type = "string"
            });

            columns.Add(new TypeSchemaColumn
            {
                Name = "Summary",
                Type = "string"
            });

            return columns;
        }

        /// <summary>
        /// Load enums.
        /// </summary>
        private bool LoadTypes(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading types.");

            var enumFiles = Directory.GetFiles(basePath, ALL_TYPE_FILES, SearchOption.AllDirectories);
            var schemaFiles = Directory.GetFiles(basePath, ALL_SCHEMA_FILE, SearchOption.AllDirectories);

            _typeRowDict.Clear();

            // add primitive types.
            var primitiveTypes = Initialize_PrimitiveTypes();

            foreach (var pair in primitiveTypes)
            {
                if (false == _primitiveSchemaDict.TryAdd(pair.Key, pair.Value))
                {
                    return false;
                }
            }

            // add enum types.
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
                var rowDatas = _jsonSerializer.Deserialize<List<TypeSchemaRow>>(rowText);

                // Create TypeBuilder.
                UpdateEnumBuiler(enumName, rowDatas);

                var eb = _ebDict[enumName];
                if (null == eb)
                {
                    return false;
                }

                // Update TypeBuilder.
                var enumType = eb.CreateType();

                // Read schema.
                var schemaText = File.ReadAllText(schemaFile);
                var schema = _jsonSerializer.Deserialize<TypeSchema>(schemaText);

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
        /// Remove enum buidler.
        /// </summary>
        private void RemoveEnumBuiler(string typeName)
        {
            // Reset module builder.
            _moduleBuilder = CreateModuleBuilder();

            foreach (var pair in _typeRowDict)
            {
                // 삭제 Enum 제외한 다른 Enum builder 갱신.
                if (!pair.Key.Equals(typeName))
                {
                    CreateEnumBuilder(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Update enum buidler.
        /// </summary>
        private void UpdateEnumBuiler(string typeName, List<TypeSchemaRow> rows)
        {
            // check existance of type.
            if (default != FindTypeOrDefault(typeName))
            {
                // Reset module builder.
                _moduleBuilder = CreateModuleBuilder();

                foreach (var pair in _typeRowDict)
                {
                    // 업데이트 Enum 제외한 다른 Enum builder 갱신.
                    if (!pair.Key.Equals(typeName))
                    {
                        CreateEnumBuilder(pair.Key, pair.Value);
                    }
                }
            }

            // Define type.
            CreateEnumBuilder(typeName, rows);
        }

        /// <summary>
        /// Create enum builder.
        /// </summary>
        private void CreateEnumBuilder(string typeName, List<TypeSchemaRow> rows)
        {
            var eb = _moduleBuilder.DefineEnum(typeName, TypeAttributes.Public, typeof(int));

            // Id.
            foreach (var row in rows)
            {
                eb.DefineLiteral(row.Name, row.Id);
            }

            // Set row, builder.
            SetEnumBuilder(typeName, eb);
            SetRows(typeName, rows);
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
        }

        /// <summary>
        /// Set rows for type.
        /// </summary>
        private void SetRows(string typeName, List<TypeSchemaRow> list)
        {
            if (false == _typeRowDict.ContainsKey(typeName))
            {
                _typeRowDict.TryAdd(typeName, null);
            }

            _typeRowDict.AddOrUpdate(typeName, list, (key, old) => list);
        }

        /// <summary>
        /// Get module builder.
        /// </summary>
        private ModuleBuilder CreateModuleBuilder()
        {
            var abName = new AssemblyName(typeof(TypeDataManager).Name);
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(abName, AssemblyBuilderAccess.Run);

            return _assemblyBuilder.DefineDynamicModule(abName.Name);
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        private TypeSchema SetSchema(string typeName, TypeSchema schema)
        {
            return _schemaDict.AddOrUpdate(typeName, schema, (key, old) => schema);
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        private bool RemoveSchema(string typeName)
        {
            var schema = FindSchemaOrDefault(typeName);
            if (default == schema)
            {
                return false;
            }

            return _schemaDict.Remove(typeName, out schema);
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

        /// <summary>
        /// Remove files
        /// </summary>
        private void RemoveFiles(string typeName)
        {
            // remvoe data file.
            var enumFile = Path.Combine(_srcFolderPath, $"{typeName}.type");
            File.Delete(enumFile);

            // remvoe schema file.
            var schemaFile = Path.Combine(_srcFolderPath, $"{typeName}.schema");
            File.Delete(schemaFile);
        }
    }
}
