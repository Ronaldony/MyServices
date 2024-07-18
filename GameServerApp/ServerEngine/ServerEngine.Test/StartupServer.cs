using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServerEngine.Test
{
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services.Interfaces;
    using System.Dynamic;

    internal class StartupServer
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ConfigureAsync()
        {
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();

            // Get appsettins.
            var configConsul = configuration.GetSection("Config_Consul").Get<Config_Consul>();

            var consulConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            consulConfigureService.Initialize(configConsul);

            var consulGame = await consulConfigureService.GetConfigData<ConsulGame>("Development");


            /////////////////////////////////////////////////////////////////////////////
            // Initialize servivces.
            var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            snowFlakeService.Initialize(consulGame.SnowflakeBaseTime, 1, 2);

            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();
        }
    }
}
