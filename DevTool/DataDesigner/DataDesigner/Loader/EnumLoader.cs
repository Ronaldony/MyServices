using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataDesigner.Loader
{
    /// <summary>
    /// EnumLoader.
    /// </summary>
    internal sealed class EnumLoader : IDataLoader
    {
        private readonly string _basePath;

        private readonly ILogger<EnumLoader> _logger;

        /// <summary>
        /// Data dictionary.
        /// key: enum name.
        /// value: enum datas string.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> _dataDict;

        public EnumLoader(IServiceProvider serviceProvider, string basePath)
        {
            _basePath = basePath;

            _logger = serviceProvider.GetRequiredService<ILogger<EnumLoader>>();

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
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[EnumLoader] Load datas.");

            var files = Directory.GetFiles(_basePath, "*.type", SearchOption.AllDirectories);
            
            _dataDict.Clear();

            foreach (var file in files)
            {
                _logger.LogInformation($"[EnumLoader] Load file: {file}");

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
