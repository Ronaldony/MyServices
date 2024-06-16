using Microsoft.Extensions.DependencyInjection;

namespace ServerEngine.Test
{
    using Microsoft.Extensions.Configuration;
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services;
    using ServerEngine.Core.Services.Interfaces;
    using Snowflake;

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

            // Consul.
            var configConsul = configuration.GetSection("Config_Console").Get<Config_Consul>();

            //var consulConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            //consulConfigureService.Initialize(configConsul);

            //var consulGame = await consulConfigureService.GetConfigData<ConsulGame>("Development");
            
            var nowTime = DateTime.UtcNow;

            var snowFlakeService = _serviceProvider.GetRequiredService<ISnowflakeService>();
            snowFlakeService.Initialize(1, 2);

            for (int cnt = 0; cnt < 20; cnt++)
            {
                Console.WriteLine($"{cnt} / Id: {snowFlakeService.GenerateId()}");
            }
        }
    }
}
