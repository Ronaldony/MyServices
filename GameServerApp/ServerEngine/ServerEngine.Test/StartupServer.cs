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
            var consulConfigureService = _serviceProvider.GetRequiredService<IRemoteConfigureService>();
            consulConfigureService.Initialize();
            
            var consulGame = await consulConfigureService.GetConfigData<ConsulGame>("Development");
        }
    }
}
