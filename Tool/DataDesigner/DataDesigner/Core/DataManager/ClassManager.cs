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
    internal sealed class ClassManager
    {
        private const string ALL_FILES = "*.obj";
        private const string FILE_DIR = "DataDesinger/Object";

        private readonly ILogger<ClassManager> _logger;

        private string _basePath;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<ClassMember>> _classMemberDict;

        public ClassManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassManager>>();

            _classMemberDict = new Dictionary<string, List<ClassMember>>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(string basePath)
        {
            _basePath = basePath;

            // Load class.
            LoadClasses(basePath);

            _logger.LogInformation($"ClassManager initialized.");
        }

        /// <summary>
        /// Update class members.
        /// </summary>
        public bool UpdateMembers(string name, List<ClassMember> members)
        {
            if (_classMemberDict.ContainsKey(name))
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
            var filePath = Path.Combine(_basePath, FILE_DIR, $"{name}.obj");
            File.WriteAllText(filePath, membersJson);

            _classMemberDict[name] = members;

            return true;
        }

        /// <summary>
        /// Create scheme for class.
        /// </summary>
        public void UpdateScheme(ClassScheme scheme)
        {
            if (false == _classMemberDict.ContainsKey(scheme.Name))
            {
                _classMemberDict.Add(scheme.Name, new List<ClassMember>());
            }

            // Check directory.
            Directory.CreateDirectory(Path.Combine(_basePath, FILE_DIR));

            // Scheme.
            var schemeFilePath = Path.Combine(_basePath, FILE_DIR, $"{scheme.Name}.scheme");
            var schemeJson = JsonConvert.SerializeObject(scheme);

            // Overwrite scheme file.
            File.WriteAllText(schemeFilePath, schemeJson);
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        private void LoadClasses(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading classes.");

            var files = Directory.GetFiles(basePath, ALL_FILES, SearchOption.AllDirectories);

            _classMemberDict.Clear();

            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);
                var className = Path.GetFileNameWithoutExtension(file);

                var classMembers = JsonConvert.DeserializeObject<List<ClassMember>>(fileContent);

                _classMemberDict.Add(className, classMembers);

                _logger.LogDebug($"Load class: {className}");
            }
        }
    }
}
