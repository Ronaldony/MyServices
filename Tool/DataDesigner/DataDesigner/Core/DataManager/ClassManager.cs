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
        private const string FILE_DIR = $"DataDesigner/{FilePath.FOLDER_CLASS}";

        private readonly ILogger<ClassManager> _logger;

        private string _srcFolderPath;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>s
        /// <returns></returns>
        private Dictionary<string, string> _classValues;

        public ClassManager(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassManager>>();

            _classValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(string basePath)
        {
            _srcFolderPath = Path.Combine(basePath, FILE_DIR);

            // Load class.
            LoadClasses(basePath);

            _logger.LogInformation($"ClassManager initialized.");
        }

        /// <summary>
        /// Get all classes.
        /// </summary>
        public Dictionary<string, string> GetAllClasses()
        {
            return _classValues;
        }

        /// <summary>
        /// Update class members.
        /// </summary>
        public bool UpdateMembers(string name, string members)
        {
            if (false == _classValues.ContainsKey(name))
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

            // Overwrite file.
            var filePath = Path.Combine(_srcFolderPath, $"{name}.obj");
            File.WriteAllText(filePath, members);

            _classValues[name] = members;

            return true;
        }

        /// <summary>
        /// Create schema for class.
        /// </summary>
        public void UpdateSchemaInfo(ClassSchemaInfo schemaInfo)
        {
            if (false == _classValues.ContainsKey(schemaInfo.Name))
            {
                _classValues.Add(schemaInfo.Name, string.Empty);
            }

            // Check directory.
            Directory.CreateDirectory(_srcFolderPath);

            // Schema.
            var schemaFilePath = Path.Combine(_srcFolderPath, $"{schemaInfo.Name}.schema");
            var schemaInfoJson = JsonConvert.SerializeObject(schemaInfo, Formatting.Indented);

            // Overwrite schema file.
            File.WriteAllText(schemaFilePath, schemaInfoJson);
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        private void LoadClasses(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"// Loading classes.");

            var files = Directory.GetFiles(basePath, ALL_FILES, SearchOption.AllDirectories);

            _classValues.Clear();

            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);
                var className = Path.GetFileNameWithoutExtension(file);

                _classValues.Add(className, fileContent);

                _logger.LogDebug($"Load class: {className}");
            }
        }
    }
}
