using PowerArgs;

namespace ServerEngine.Test
{
    using ServerEngine.AsciiBanner;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        private const string PROGRAM_NAME = "ServerEngine.Test"; 

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

            var startupServer = new StartupServer(startup.WebApp.Services);
            await startupServer.ConfigureAsync();

            var asciiBannerWriter = new AsciiBannerWriter("slant.flf");
            var logger = startup.WebApp.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation(asciiBannerWriter.GetAsciiText(PROGRAM_NAME));

            TestCode();

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

        /// <summary>
        /// Test code.
        /// </summary>
        private static async Task TestCode()
        {
            var observable = Observable.Interval(TimeSpan.FromSeconds(1));

            var enumerable = observable.ToAsyncEnumerable();

            await foreach(var ss in enumerable)
            {
                Console.WriteLine(ss);
            }

        }
    }
}