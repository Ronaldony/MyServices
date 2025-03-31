
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PowerArgs;

namespace Server_World
{
    using GameCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServerEngine.AsciiBanner;
    using System.Diagnostics;
    using System.Numerics;
    using System.Text;

    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        public const string PROGRAM_NAME = "Server_World";

        public static async Task Main(string[] args)
        {
            var arguments = ParseCommand(args);
            if (arguments == null)
            {
                Console.WriteLine("Arguments is null.");
                return;
            }

            // Host builder.
            var hostBuilder = Host.CreateApplicationBuilder();

            hostBuilder.Configuration.AddJsonFile(arguments.ConfigFile, optional: false, reloadOnChange: false);

            // Startup.
            var startup = new Startup();
            startup.Configure(hostBuilder);

            // Host.
            var host = hostBuilder.Build();

            var startupServer = new StartupServer(host.Services, arguments);
            await startupServer.ConfigureAsync();

            var asciiBannerWriter = new AsciiBannerWriter("slant.flf");
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation(asciiBannerWriter.GetAsciiText(PROGRAM_NAME));

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