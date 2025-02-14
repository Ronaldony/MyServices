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
        /// TypeGenerateGroup dictionary.
        /// key: type name.
        /// value: TypeGenerateGroup.
        /// </summary>
        private ConcurrentDictionary<string, GenerateTypeGroup> _typeGroupDict;

        /// <summary>
        /// key: type name.
        /// value: schema string.
        /// </summary>
        private ConcurrentDictionary<string, TypeSchema> _primitiveSchemaDict;

        public TypeDataManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<TypeDataManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _typeSchemaColumns = new List<TypeSchemaColumn>();

            _typeGroupDict = new ConcurrentDictionary<string, GenerateTypeGroup>();
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

            // initialize type columns.
            _typeSchemaColumns.Clear();
            _typeSchemaColumns.Add(new TypeSchemaColumn { Name = "Id", Type = "int" });
            _typeSchemaColumns.Add(new TypeSchemaColumn { Name = "Name", Type = "string" });
            _typeSchemaColumns.Add(new TypeSchemaColumn { Name = "Summary", Type = "string" });

            _logger.LogInformation($"EnumManager initialized.");

            return true;
        }

        /// <summary>
        /// Get types.
        /// </summary>
        public Type FindTypeOrDefault(string typeName)
        {
            // check key.
            if (false == _typeGroupDict.ContainsKey(typeName))
            {
                return default;
            }

            // check builer.
            if (default == _typeGroupDict[typeName].Builder)
            {
                return default;
            }

            return _typeGroupDict[typeName].Builder.CreateType();
        }

        /// <summary>
        /// Get enum types.
        /// </summary>
        public IEnumerable<Type> GetEnumTypes()
        {
            return _typeGroupDict.Values.Select(d => d.Builder?.CreateType());
        }

        /// <summary>
        /// Get schema names with primitive types(int, float, bool, string etc..).
        /// </summary>
        public IEnumerable<string> GetAllType()
        {
            var typeSchemas = new List<string>();

            typeSchemas.AddRange(_primitiveSchemaDict.Keys);
            typeSchemas.AddRange(_typeGroupDict.Keys);

            return typeSchemas;
        }

        /// <summary>
        /// Get schema for type.
        /// </summary>
        public TypeSchema FindSchemaOrDefault(string schemaName)
        {
            if (false == _typeGroupDict.ContainsKey(schemaName))
            {
                return default;
            }

            return _typeGroupDict[schemaName].Schema;
        }

        /// <summary>
        /// Get rows for type.
        /// </summary>
        public List<TypeSchemaRow> GetRowsOrDefault(string typeName)
        {
            if (false == _typeGroupDict.ContainsKey(typeName))
            {
                return default;
            }

            return _typeGroupDict[typeName].Rows;
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
        /// Load enums.
        /// </summary>
        private bool LoadTypes(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading types.");

            var dataFiles = Directory.GetFiles(basePath, ALL_TYPE_FILES, SearchOption.AllDirectories);
            var schemaFiles = Directory.GetFiles(basePath, ALL_SCHEMA_FILE, SearchOption.AllDirectories);

            _typeGroupDict.Clear();

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
            foreach (var file in dataFiles)
            {
                var enumName = Path.GetFileNameWithoutExtension(file);

                // Schema file.
                var schemaFile = schemaFiles.FirstOrDefault(d => Path.GetFileNameWithoutExtension(d).Equals(enumName));
                if (string.IsNullOrEmpty(schemaFile))
                {
                    return false;
                }

                // Read schema.
                var schemaText = File.ReadAllText(schemaFile);
                var schema = _jsonSerializer.Deserialize<TypeSchema>(schemaText);

                // Read rows.
                var rowText = File.ReadAllText(file);
                var rowDatas = _jsonSerializer.Deserialize<List<TypeSchemaRow>>(rowText);

                // Enum builder.
                var eb = CreateEnumBuilder(enumName, rowDatas);
                if (null == eb)
                {
                    return false;
                }

                // Add GenerateTypeGroup.
                var typeGroup = new GenerateTypeGroup
                {
                    Rows = rowDatas,
                    Builder = eb,
                    Schema = schema,
                };

                AddOrUpdateTypeGroup(typeGroup);

                _logger.LogDebug($"Load enum: {enumName}");
            }

            return true;
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private void AddOrUpdateTypeGroup(GenerateTypeGroup typeGroup)
        {
            var typeName = typeGroup.Builder.Name;

            _typeGroupDict.AddOrUpdate(typeName, typeGroup, (key, old) => typeGroup);
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private void AddOrUpdateTypeGroup(string typeName, List<TypeSchemaRow> rows = default, EnumBuilder tb = default, TypeSchema schema = default)
        {
            // Check type group.
            if (false == _typeGroupDict.ContainsKey(typeName))
            {
                var newTypeGroup = new GenerateTypeGroup();
                _typeGroupDict.AddOrUpdate(typeName, newTypeGroup, (key, old) => newTypeGroup);
            }

            // Update if data is not default.
            var typeGroup = _typeGroupDict[typeName];

            if (default != rows)
            {
                typeGroup.Rows = rows;
            }

            if (default != tb)
            {
                typeGroup.Builder = tb;
            }

            if (default != schema)
            {
                typeGroup.Schema = schema;
            }

            _typeGroupDict.AddOrUpdate(typeName, typeGroup, (key, old) => typeGroup);
        }

        /// <summary>
        /// Add or update type group.
        /// </summary>
        private bool RemoveTypeGroup(string typeName)
        {
            if (false == _typeGroupDict.ContainsKey(typeName))
            {
                return false;
            }

            var findTypeGroup = _typeGroupDict[typeName];

            return _typeGroupDict.Remove(typeName, out findTypeGroup);
        }

        /// <summary>
        /// Remove enum buidler.
        /// </summary>
        private void RemoveType(string typeName)
        {
            RefreshEnumBuilder(typeName);
        }

        /// <summary>
        /// Update enum buidler.
        /// </summary>
        private void UpdateType(string typeName, List<TypeSchemaRow> rows)
        {
            // check existance of type.
            if (default != FindTypeOrDefault(typeName))
            {
                RefreshEnumBuilder(typeName);
            }

            // Define type.
            var eb = CreateEnumBuilder(typeName, rows);

            // Update enum builder.
            AddOrUpdateTypeGroup(typeName, rows: rows, tb: eb);
        }

        /// <summary>
        /// Create enum builder.
        /// </summary>
        private EnumBuilder CreateEnumBuilder(string typeName, List<TypeSchemaRow> rows)
        {
            try
            {
                var eb = _moduleBuilder.DefineEnum(typeName, TypeAttributes.Public, typeof(int));

                // Id.
                foreach (var row in rows)
                {
                    eb.DefineLiteral(row.Name, row.Id);
                }

                return eb;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Refresh 
        /// </summary>
        private void RefreshEnumBuilder(string exceptTypeName = null)
        {
            // Reset module builder.
            _moduleBuilder = CreateModuleBuilder();

            foreach (var pair in _typeGroupDict)
            {
                var typeGroup = pair.Value;
                
                if (false == pair.Key.Equals(exceptTypeName))
                {
                    var eb = CreateEnumBuilder(pair.Key, typeGroup.Rows);
                    AddOrUpdateTypeGroup(pair.Key, tb: eb);
                }
            }
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
