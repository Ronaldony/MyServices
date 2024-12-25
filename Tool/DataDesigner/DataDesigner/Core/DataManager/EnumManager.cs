using Newtonsoft.Json;

namespace DataDesigner.Core.DataManager
{
    using DataDesigner.Core.Data;

    /// <summary>
    /// EnumLoader.
    /// Description: Load enum datas from .type format files.
    /// </summary>
    internal sealed class EnumManager
    {
        private const string ALL_FILES = "*.type";
        private const string FILE_DIR = $"DataDesigner/{FilePath.FOLDER_ENUM}";
        
        private string _srcFolderPath;

        private readonly ILogger<EnumManager> _logger;

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

            _enumMemberDict = new Dictionary<string, List<EnumMember>>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, FILE_DIR);

            // Load enum datas.
            LoadEnums(basePath);

            _logger.LogInformation($"EnumManager initialized.");
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

            var membersJson = JsonConvert.SerializeObject(members, Formatting.Indented);

            // Overwrite file.
            var filePath = Path.Combine(_srcFolderPath, $"{name}.type");
            File.WriteAllText(filePath, membersJson);

            _enumMemberDict[name] = members;

            return true;
        }

        /// <summary>
        /// Update shceme for type.
        /// </summary>
        public void UpdateSchemaInfo(EnumSchemaInfo schemaInfo)
        {
            if (false == _enumMemberDict.ContainsKey(schemaInfo.Name))
            {
                _enumMemberDict.Add(schemaInfo.Name, new List<EnumMember>());
            }

            // Check directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Create schema file.
            var schemaFilePath = Path.Combine(_srcFolderPath, $"{schemaInfo.Name}.schema");
            var schemaJson = JsonConvert.SerializeObject(schemaInfo, Formatting.Indented);

            File.WriteAllText(schemaFilePath, schemaJson);
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        private void LoadEnums(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading enum.");

            var files = Directory.GetFiles(basePath, ALL_FILES, SearchOption.AllDirectories);

            _enumMemberDict.Clear();

            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);
                var enumName = Path.GetFileNameWithoutExtension(file);

                var enumMembers = JsonConvert.DeserializeObject<List<EnumMember>>(fileContent);

                _enumMemberDict.Add(enumName, enumMembers);

                _logger.LogDebug($"Load enum: {enumName}");
            }
        }
    }
}
