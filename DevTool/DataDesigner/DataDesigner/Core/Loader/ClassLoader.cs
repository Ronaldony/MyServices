using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataDesigner.Core.Loader
{
    using DataDesigner.Core.DataMember;

    /// <summary>
    /// EnumLoader.
    /// Description: Load enum datas from .type format files.
    /// </summary>
    internal sealed class ClassLoader
    {
        private string ALL_FILES = "*.obj";

        private readonly ILogger<ClassLoader> _logger;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<ClassMember>> _classMemberDict;

        public ClassLoader(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<ClassLoader>>();

            _classMemberDict = new Dictionary<string, List<ClassMember>>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation($"EnumLoader initialized.");
        }

        /// <summary>
        /// Load datas.
        /// </summary>
        public void LoadEnumDatas(string basePath)
        {
            _logger.LogDebug($"//-------------------------------------------");
            _logger.LogDebug($"[EnumLoader] Loading enum.");

            var files = Directory.GetFiles(basePath, ALL_FILES, SearchOption.AllDirectories);

            _classMemberDict.Clear();

            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);
                var className = Path.GetFileNameWithoutExtension(file);

                var classMembers = JsonConvert.DeserializeObject<List<ClassMember>>(fileContent);

                _classMemberDict.Add(className, classMembers);

                _logger.LogDebug($"Load enum: {className}");
            }
        }

        /// <summary>
        /// Updae type.
        /// </summary>
        public bool Update(string name, List<ClassMember> members)
        {
            if (_classMemberDict.ContainsKey(name))
            {
                return false;
            }

            _classMemberDict[name] = members;

            return true;
        }

        /// <summary>
        /// Add type.
        /// </summary>
        public bool CreateScheme(ClassScheme scheme)
        {
            if (_classMemberDict.ContainsKey(scheme.Name))
            {
                return false;
            }

            _classMemberDict.Add(scheme.Name, new List<ClassMember>());

            return true;
        }
    }
}
