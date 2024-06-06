
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerArgs;
using ServerEngine.Core.Services;
using ServerEngine.Core.Services.Interfaces;

namespace ServerEngine.Test
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var arguments = ParseCommand(args);
            if (arguments == null)
            {
                Console.WriteLine("Arguments is null.");
                return;
            }

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configs =>
                {
                    configs.AddJsonFile(arguments.ConfigFile);
                    configs.SetBasePath(AppContext.BaseDirectory);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IRemoteConfigureService, ConsulConfigureService>();
                })
                .Build();

            var startupServer = new StartupServer(host.Services);
            await startupServer.ConfigureAsync();

            host.Run();
        }

        private static Arguments ParseCommand(string[] args)
        {
            try
            {
                var arguments = Args.Parse<Arguments>(args);
                return arguments;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}