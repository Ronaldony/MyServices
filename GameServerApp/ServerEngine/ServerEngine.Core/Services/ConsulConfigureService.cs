using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Config;
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// ConsulConfigureService.
    /// Configuration service using consul from vault.
    /// </summary>
    public sealed class ConsulConfigureService : IRemoteConfigureService
    {
        private readonly ILogger<ConsulConfigureService> _logger;
        
        private readonly IJsonSerializer _jsonSerializer;

        private ConsulClient _consulClient;

        public ConsulConfigureService(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<ConsulConfigureService>();

            _jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();
        }

        /// <summary>
        /// Initialize configure datas.
        /// </summary>
        public bool Initialize(ConfigBase config)
        {
            var configConsul = config as Config_Consul;

            if (configConsul == null)
            {
                _logger.LogError("ConfigBase is null.");
                return false;
            }

            _consulClient = new ConsulClient(new ConsulClientConfiguration
            {
                Address = new Uri(configConsul.Address),
                Datacenter = configConsul.Datacenter,
            });

            _logger.LogInformation("ConsulConfigureService initialize.");

            return true;
        }

        /// <summary>
        /// Get configuration data.
        /// </summary>
        public async Task<T> GetConfigData<T>(string key) where T : class
        {
            using (var client = new ConsulClient(_consulClient.Config))
            {
                try
                {
                    var getPair = await client.KV.Get(key);
                    var keyValue = Encoding.UTF8.GetString(getPair.Response.Value);
                    
                    return _jsonSerializer.Deserialize<T>(keyValue);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
            }
        }
    }
}
