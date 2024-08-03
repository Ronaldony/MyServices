using PowerArgs;
using ServerEngine.Core.Util;
using ServerEngine.Test.Database.Data;
using ServerEngine.Test.Database.DataObject;

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

            TestCode();

            var startup = new Startup(args, arguments);
            startup.Configure();

            var startupServer = new StartupServer(startup.WebApp.Services);
            await startupServer.ConfigureAsync();

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

        public static void TestCode()
        {
        }
    }
}