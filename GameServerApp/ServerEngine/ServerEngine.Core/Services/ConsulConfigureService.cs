using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace ServerEngine.Core.Services
{
    using Config;
    using Interfaces;
    using ServerEngine.Config.Consul;

    /// <summary>
    /// ConsulConfigureService.
    /// Configuration service using consul from vault.
    /// </summary>
    public sealed class ConsulConfigureService : IRemoteConfigureService
    {
        private readonly ILogger<ConsulConfigureService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private ConsulClient _consulClient;

        public ConsulConfigureService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<ConsulConfigureService>();
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
                    
                    return JsonConvert.DeserializeObject<T>(keyValue);
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
