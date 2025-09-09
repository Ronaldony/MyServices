
using Dapper;
using Dapper.ColumnMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Config.Database;
using ServerEngine.Config.Strapi;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.MessageBus.RabbitMQs;
using System.Reactive.Linq;
using System.Reflection;

namespace Server_Control
{
    using Framework.Config.ConfigManager;
    using Framework.Config.Configs;
    using Framework.Config.MessageBus;
    using Framework.Core.Database.Attributes;
    using Framework.Core.Utilities;
    using GameCore.Interfaces;
    using Server_Control.Core.Services;
    using Server_Control.Server.Hub;

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
            // Utilies.
            ServiceUtility.Initialize(_serviceProvider);

            var configuration = _serviceProvider.GetService<IConfiguration>();

            // Get appsettins.
            var configStrapi = configuration.GetSection("Config_Strapi").Get<Config_Strapi>();

			/// RemoteConfigureService.
			var remoteConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            remoteConfigureService.Initialize(configStrapi);

            // Configure.
            var configGame = await remoteConfigureService.GetConfigData<Config_Game>(configStrapi.ContentType, "Devel_Game");
            var mysqlConnections = await remoteConfigureService.GetConfigData<List<Config_MySQLConnection>>(configStrapi.ContentType, "Devel_MySQL");
            var couchbaseConnections = await remoteConfigureService.GetConfigData<List<Config_CouchbaseConnection>>(configStrapi.ContentType, "Devel_Couchbase");
            var messageBusDatas = await remoteConfigureService.GetConfigData<List<Config_MessageBusData>>(configStrapi.ContentType, "Devel_MessageBus");

            GameConfigManager.Initialize(configGame, mysqlConnections, couchbaseConnections, messageBusDatas);

            // MemoryPoolService
            var memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            memoryPoolService.Initialize();

            // IJsonSerializer.
            var jsonSerializer = _serviceProvider.GetRequiredService<IJsonSerializer>();
            jsonSerializer.Initialize();

            // ITDBService.
            var tdbService = _serviceProvider.GetRequiredService<ITDBService>();
            tdbService.Initialize();

            /////////////////////////////////////////////////////////////////////////////
            // Initialize servivces.
            var snowFlakeService = _serviceProvider.GetRequiredService<IUniqueIdService>();
            snowFlakeService.Initialize(configGame.SnowflakeBaseTime, _arguments.DataCenter, _arguments.ServerId);

            // rbmqConnection.
            var rbmqConnection = _serviceProvider.GetRequiredService<RBMQConnection>();
            var confMessageBus = GameConfigManager.ConfigGame.MessageBus;

            await rbmqConnection.InitializeAsync(confMessageBus.Host, confMessageBus.User, confMessageBus.Pass, ServiceUtility.GetCTS().Token);

            /////////////////////////////////////////////////////////////////////////////
            /// Background.

            // postService.
            var postService = _serviceProvider.GetRequiredService<PostService>();
            postService.Initialize();

            // Database data mapping.
            MappingDapper();

            // Start servers.
            BootstrapServer();
        }

        /// <summary>
        /// Mapping dapper.
        /// </summary>
        private void MappingDapper()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var asm in assemblies)
            {
                var types = asm.GetTypes().Where(t => t.GetCustomAttribute<DapperMappingAttribute>() != null);

                foreach (var type in types)
                {
                    SqlMapper.SetTypeMap(type, new ColumnTypeMapper(type));
                }
            }
        }

        /// <summary>
        /// Bootstrap server.
        /// </summary>
        private void BootstrapServer()
        {
            // Start server.
            var hubServer = _serviceProvider.GetRequiredService<HubServer>();
            hubServer.Start<HubHanlder>(_arguments.Port_PlayHub, false);
        }
    }
}
