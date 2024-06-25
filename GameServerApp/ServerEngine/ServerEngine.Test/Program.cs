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
        public static async Task Main(string[] args)
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
                    services.AddSingleton<ISnowflakeService, SnowflakeService>();
                    services.AddSingleton<IDataSerializer, MemoryPackDataSerializer>();
                    services.AddSingleton<IJsonSerializer, CustomJsonSerializer>();
                })
                .ConfigureAppConfiguration(services =>
                {
                })
                .Build();

            var startupServer = new StartupServer(host.Services);
            await startupServer.ConfigureAsync();

            await host.RunAsync();
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