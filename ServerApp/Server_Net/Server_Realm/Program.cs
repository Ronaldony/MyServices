
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerArgs;

namespace Server_Realm
{
    using ServerEngine.AsciiBanner;

    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        public const string PROGRAM_NAME = "Server_Realm";

        public static async Task Main(string[] args)
        {
            var arguments = ParseCommand(args);
            if (arguments == null)
            {
                Console.WriteLine("Arguments is null.");
                return;
            }

            // Setup global.
            SetupGlobal(arguments);

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

        /// <summary>
        /// Setup global.
        /// </summary>
        private static void SetupGlobal(Arguments arg)
        {
            Global.Server = arg.Name;
            Global.DataCenter = arg.DataCenter;
            Global.ServerId = arg.ServerId;
            Global.HuntZoneId = $"HuntZone_{Global.ServerId}";
        }
    }
}