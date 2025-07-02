using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Config;
    using ServerEngine.Config.Strapi;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// StrapiConfigureService.
    /// </summary>
    public sealed class StrapiConfigureService : IRemoteConfigureService
    {
        private const string STRAPI_CONTENT_PATH = "/api/config-project-rpgs";

        private readonly ILogger<StrapiConfigureService> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        private Config_Strapi _configStrapi;

        /// <summary>
        /// Strapi datas.
        /// </summary>
        private Dictionary<string, Config_StrapiData> _configStrapiDict;

        public StrapiConfigureService(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<StrapiConfigureService>();

            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();

            _configStrapiDict = new Dictionary<string, Config_StrapiData>();
        }

        /// <summary>
        /// Initialize configure datas.
        /// </summary>
        public bool Initialize(ConfigBase config)
        {
            _configStrapi = config as Config_Strapi;

            if (_configStrapi == null)
            {
                _logger.LogError("fail to initialize StrapiConfigureService.");
                return false;
            }

            _logger.LogInformation("StrapiConfigureService initialize.");

            return true;
        }

        /// <summary>
        /// Get configuration data.
        /// </summary>
        public async Task<T> GetConfigData<T>(string contentType, string nameKey) where T : class
        {
            try
            {
                if (_configStrapiDict.ContainsKey(nameKey))
                {
                    return _jsonSerializer.Deserialize<T>(_configStrapiDict[nameKey].Configuration.ToString());
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"bearer {_configStrapi.Token}");

                    var res = await client.GetAsync($"{_configStrapi.Address}{contentType}");
                    var resString = await res.Content.ReadAsStringAsync();

                    var strapiDatas = FilterStrapiDatas(resString);

                    foreach (var data in strapiDatas)
                    {
                        if (false == _configStrapiDict.ContainsKey(data.Name))
                        {
                            _configStrapiDict.Add(data.Name, data);
                        }

                        _configStrapiDict[data.Name] = data;
                    }

                    return _jsonSerializer.Deserialize<T>(_configStrapiDict[nameKey].Configuration.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Filter configuration.
        /// </summary>
        private List<Config_StrapiData> FilterStrapiDatas(string resString)
        {
            try
            {
                var strapiRes = _jsonSerializer.Deserialize<Config_StrapiRes>(resString);

                return strapiRes.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
