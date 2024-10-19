namespace ServerEngine.Test
{
    using ServerEngine.Config.Consul;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Test.Database.Data;

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

            var objectPoolService = _serviceProvider.GetRequiredService<IObjectPoolService>();
            objectPoolService.Initialize();

            var dataObject = objectPoolService.Acquire<DTO_PlayerInfo>();
            objectPoolService.Release(dataObject);

            dataObject = objectPoolService.Acquire<DTO_PlayerInfo>();

            //         var configGame = await consulConfigureService.GetConfigData<Config_Game>("Development");
            //         var configDatabase = await consulConfigureService.GetConfigData<List<Config_Database>>("Development_Database");
            //var configCache = await consulConfigureService.GetConfigData<IEnumerable<CacheHost>>("Development_Cache");

            //GameConfigManager.Initialize(configGame, configDatabase);

            //         /////////////////////////////////////////////////////////////////////////////
            //         // Initialize servivces.
            //         var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            //         snowFlakeService.Initialize(configGame.SnowflakeBaseTime, 1, 2);

            //         var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            //         jsonSerializer.Initialize();

            //         var memcachedService = _serviceProvider.GetRequiredService<IMemcachedService>();
            //         memcachedService.Initialize(_serviceProvider, configCache, 1000);
        }
    }
}
