using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private const string FILE_DIR = "DataDesigner/Type";
        
        private string _basePath;

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
            _basePath = basePath;

            // Load enum datas.
            LoadEnums(basePath);

            _logger.LogInformation($"EnumManager initialized.");
        }

        /// <summary>
        /// Update type.
        /// </summary>
        public bool UpdateMembers(string name, List<EnumMember> members)
        {
            if (_enumMemberDict.ContainsKey(name))
            {
                return false;
            }

            // Check directory.
            Directory.CreateDirectory(Path.Combine(_basePath, FILE_DIR));

            // Check scheme file exist.
            var schemeFilePath = Path.Combine(_basePath, FILE_DIR, $"{name}.scheme");
            if (false == File.Exists(schemeFilePath))
            {
                return false;
            }

            var membersJson = JsonConvert.SerializeObject(members, Formatting.Indented);

            // Overwrite file.
            var filePath = Path.Combine(_basePath, FILE_DIR, $"{name}.type");
            File.WriteAllText(filePath, membersJson);

            _enumMemberDict[name] = members;

            return true;
        }

        /// <summary>
        /// Update shceme for type.
        /// </summary>
        public void UpdateScheme(EnumScheme scheme)
        {
            if (false == _enumMemberDict.ContainsKey(scheme.Name))
            {
                _enumMemberDict.Add(scheme.Name, new List<EnumMember>());
            }

            // Check directory.
            Directory.CreateDirectory(Path.Combine(_basePath, FILE_DIR));

            // Create scheme file.
            var schemeFilePath = Path.Combine(_basePath, FILE_DIR, $"{scheme.Name}.scheme");
            var schemeJson = JsonConvert.SerializeObject(scheme);

            File.WriteAllText(_basePath, schemeJson);
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
