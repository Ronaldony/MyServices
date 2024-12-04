using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataDesigner.Loader
{
    /// <summary>
    /// ObjectLoader.
    /// </summary>
    internal sealed class ObjectLoader : IDataLoader
    {
        private readonly string _basePath;

        private readonly ILogger<ObjectLoader> _logger;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> _dataDict;

        public ObjectLoader(IServiceProvider serviceProvider, string basePath)
        {
            _basePath = basePath;

            _logger = serviceProvider.GetRequiredService<ILogger<ObjectLoader>>();

            _dataDict = new Dictionary<string, string>();
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
        public void LoadAllDatas()
        {
            var files = Directory.GetFiles(_basePath, "*.obj", SearchOption.AllDirectories);
            
            _dataDict.Clear();

            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);

                var fileName = Path.GetFileNameWithoutExtension(file);
                _dataDict.Add(fileName, fileContent);
            }
        }

        /// <summary>
        /// Get data dictionary.
        /// </summary>
        public IDictionary<string, string> GetDataDict()
        {
            return _dataDict;
        }
    }
}
