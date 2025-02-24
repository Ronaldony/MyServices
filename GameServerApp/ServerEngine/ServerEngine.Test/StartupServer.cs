
#define TEST_LOCAL

namespace ServerEngine.Test
{
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.Interfaces;

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

			/// RemoteConfigureService.
			var remoteConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            remoteConfigureService.Initialize(configConsul);

			// MemoryPoolService
			var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // TestServer
            var testServer = _serviceProvider.GetRequiredService<IUdpServer>();
            testServer.Start(9050, false);

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();

#if! TEST_LOCAL
            var configGame = await consulConfigureService.GetConfigData<Config_Game>("Development");
            var configDatabase = await consulConfigureService.GetConfigData<List<Config_Database>>("Development_Database");
            var configCache = await consulConfigureService.GetConfigData<IEnumerable<CacheHost>>("Development_Cache");

            GameConfigManager.Initialize(configGame, configDatabase);

            /////////////////////////////////////////////////////////////////////////////
            // Initialize servivces.
            var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            snowFlakeService.Initialize(configGame.SnowflakeBaseTime, 1, 2);

            var memcachedService = _serviceProvider.GetRequiredService<IMemcachedService>();
            memcachedService.Initialize(_serviceProvider, configCache, 1000);
#endif
        }
    }
}
