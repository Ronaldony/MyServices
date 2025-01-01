namespace DataDesigner.Core.TypeManager
{
    using DataDesigner.Core.Data;
    using DataDesigner.Core.Services.Interfaces;

    /// <summary>
    /// EnumLoader.
    /// Description: Load enum datas from .type format files.
    /// </summary>
    internal sealed class EnumManager
    {
        private const string ALL_DATA_FILES = "*.type";
        private const string SRC_FOLDER = $"DataDesigner/{FilePath.FOLDER_ENUM}";
        
        private string _srcFolderPath;

        private readonly ILogger<EnumManager> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        private List<Type> _enumTypes;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<EnumMember>> _enumMemberDict;

        public EnumManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumManager>>();
            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _enumMemberDict = new Dictionary<string, List<EnumMember>>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, SRC_FOLDER);

            // Load enum datas.
            LoadEnums(basePath);

            _logger.LogInformation($"EnumManager initialized.");
        }

        /// <summary>
        /// Get enum Types.
        /// </summary>
        public IList<Type> GetTypes()
        {
            return _enumTypes;
        }

        /// <summary>
        /// Update type.
        /// </summary>
        public bool UpdateMembers(string name, List<EnumMember> members)
        {
            if (false == _enumMemberDict.ContainsKey(name))
            {
                return false;
            }

            // Check directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Check schema file exist.
            var schemaFilePath = Path.Combine(_srcFolderPath, $"{name}.schema");
            if (false == File.Exists(schemaFilePath))
            {
                return false;
            }

            var membersJson = _jsonSerializer.Serialize(members);

            // Overwrite file.
            var filePath = Path.Combine(_srcFolderPath, $"{name}.type");
            File.WriteAllText(filePath, membersJson);

            _enumMemberDict[name] = members;

            return true;
        }

        /// <summary>
        /// Update shceme for type.
        /// </summary>
        public void UpdateSchemaInfo(EnumSchemaColumn schemaInfo)
        {
            if (false == _enumMemberDict.ContainsKey(schemaInfo.Name))
            {
                _enumMemberDict.Add(schemaInfo.Name, new List<EnumMember>());
            }

            // Check directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Create schema file.
            var schemaFilePath = Path.Combine(_srcFolderPath, $"{schemaInfo.Name}.schema");
            var schemaJson = _jsonSerializer.Serialize(schemaInfo);

            File.WriteAllText(schemaFilePath, schemaJson);
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        private void LoadEnums(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading enum.");

            var files = Directory.GetFiles(basePath, ALL_DATA_FILES, SearchOption.AllDirectories);

            _enumMemberDict.Clear();

            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);
                var enumName = Path.GetFileNameWithoutExtension(file);

                var enumMembers = _jsonSerializer.Deserialize<List<EnumMember>>(fileContent);

                _enumMemberDict.Add(enumName, enumMembers);

                _logger.LogDebug($"Load enum: {enumName}");
            }
        }
    }
}
