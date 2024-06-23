using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ServerEngine.Test
{
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services.Interfaces;

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

            var snowFlakeService = _serviceProvider.GetRequiredService<ISnowflakeService>();
            snowFlakeService.Initialize(consulGame.SnowflakeBaseTime, 1, 2);

            for (int cnt = 0; cnt < 20; cnt++)
            {
                Console.WriteLine($"{cnt} / Id: {snowFlakeService.GenerateId()}");
            }
        }
    }
}
