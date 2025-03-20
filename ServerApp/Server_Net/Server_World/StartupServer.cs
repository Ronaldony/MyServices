
//#define TEST_LOCAL

using Dapper;
using Dapper.ColumnMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Server_World
{
    using Framework.Core.Database.DBO;
    using Server_World.Core.Utilities;
    using ServerEngine.Config;
    using ServerEngine.Config.ConfigManager;
    using ServerEngine.Config.Database;
    using ServerEngine.Config.Strapi;
    using ServerEngine.Core.Services.Interfaces;

    internal class StartupServer
    {
        private readonly IServiceProvider _serviceProvider;

        private Arguments _arguments;

        public StartupServer(IServiceProvider serviceProvider, Arguments arguments)
        {
            _serviceProvider = serviceProvider;

            _arguments = arguments;
        }

        public async Task ConfigureAsync()
        {
            var configuration = _serviceProvider.GetService<IConfiguration>();

            // Get appsettins.
            var configStrapi = configuration.GetSection("Config_Strapi").Get<Config_Strapi>();

			/// RemoteConfigureService.
			var remoteConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            remoteConfigureService.Initialize(configStrapi);

			// MemoryPoolService
			var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();

            // HostService.
            ServiceUtility.Initialize(_serviceProvider);

#if !TEST_LOCAL
            var configGame = await remoteConfigureService.GetConfigData<Config_Game>("Devel_Game");
            var mysqlConnections = await remoteConfigureService.GetConfigData<List<Config_MySQLConnection>>("Devel_MySQL");
            var couchbaseConnections = await remoteConfigureService.GetConfigData<List<Config_CouchbaseConnection>>("Devel_Couchbase");

            GameConfigManager.Initialize(configGame, mysqlConnections, couchbaseConnections);

            /////////////////////////////////////////////////////////////////////////////
            // Initialize servivces.
            var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            snowFlakeService.Initialize(configGame.SnowflakeBaseTime, _arguments.DataCenter, _arguments.ServerId);
#endif

            // Dapper mapping.
            SqlMapper.SetTypeMap(typeof(DBO_User), new ColumnTypeMapper(typeof(DBO_User)));
        }
    }
}
