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
    internal sealed class EnumLoader
    {
        private string ALL_FILES = "*.type";

        private readonly ILogger<EnumLoader> _logger;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<EnumMember>> _enumMemberDict;

        public EnumLoader(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumLoader>>();

            _enumMemberDict = new Dictionary<string, List<EnumMember>>();
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

        /// <summary>
        /// Updae type.
        /// </summary>
        public bool Update(string name, List<EnumMember> members)
        {
            if (_enumMemberDict.ContainsKey(name))
            {
                return false;
            }

            _enumMemberDict[name] = members;

            return true;
        }

        /// <summary>
        /// Add type.
        /// </summary>
        public bool CreateScheme(EnumScheme scheme)
        {
            if (_enumMemberDict.ContainsKey(scheme.Name))
            {
                return false;
            }

            _enumMemberDict.Add(scheme.Name, new List<EnumMember>());

            return true;
        }
    }
}
