
//#define TEST_LOCAL

namespace ServerEngine.Test
{
    using ServerEngine.Config;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Config.Strapi;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Test.Server;

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
            var configStrapi = configuration.GetSection("Config_Strapi").Get<Config_Strapi>();

			/// RemoteConfigureService.
			var remoteConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            remoteConfigureService.Initialize(configStrapi);

			// MemoryPoolService
			var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // TestServer
            var testServer = _serviceProvider.GetRequiredService<TestServer>();
            testServer.Start(9050, false);    // port, peer address changable, polling interval.

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();

#if! TEST_LOCAL
            var configGame = await remoteConfigureService.GetConfigData<Config_Game>("Development_Game");
            var configDatabase = await remoteConfigureService.GetConfigData<List<Config_Database>>("Development_Database");

            GameConfigManager.Initialize(configGame, configDatabase);

            /////////////////////////////////////////////////////////////////////////////
            // Initialize servivces.
            var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            snowFlakeService.Initialize(configGame.SnowflakeBaseTime, 1, 2);
#endif
        }
    }
}
