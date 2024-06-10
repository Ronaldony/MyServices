using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Config;
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services.Interfaces;

    public class ConsulConfigureService : IRemoteConfigureService
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

        public void Initialize(ConfigBase config)
        {
            var configConsul = config as Config_Consul;

            _consulClient = new ConsulClient(new ConsulClientConfiguration
            {
                Address = new Uri(configConsul.Address),
                Datacenter = configConsul.Datacenter,
            });

            _logger.LogInformation("ConsulConfigureService initialize.");
        }

        /// <summary>
        /// 
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
