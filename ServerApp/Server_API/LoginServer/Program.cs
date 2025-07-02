using PowerArgs;
using ServerEngine.AsciiBanner;

namespace LoginServer
{
    public class Program
    {
        private const string PROGRAM_NAME = "LoginServer";

        public static async Task Main(string[] args)
        {
            var arguments = ParseCommand(args);
            if (arguments == null)
            {
                Console.WriteLine("Arguments is null.");
                return;
            }

            var startup = new Startup(args, arguments);
            startup.Configure();

            var services = startup.WebApp.Services;

            var startupServer = new StartupServer(services, arguments);
            await startupServer.ConfigureAsync();

            var asciiBannerWriter = new AsciiBannerWriter("slant.flf");
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation(asciiBannerWriter.GetAsciiText(PROGRAM_NAME));

            await startup.WebApp.RunAsync();
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
