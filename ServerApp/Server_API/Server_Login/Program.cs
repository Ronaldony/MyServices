using Server_Login.Controllers.Post;
using PowerArgs;
using ServerEngine.AsciiBanner;
using ServerEngine.Core.Services.Interfaces;

namespace Server_Login
{
    public class Program
    {
        private const string PROGRAM_NAME = "Server_Login";

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


            var postItem = new PostItem();
            postItem.Pid = "06640336521C0000";
            postItem.Items = new List<PostItemData>();
            var data = new PostItemData();
            data.RefId = 100;
            data.Count = 10;
            postItem.Items.Add(data);

            var jsonSerializer = services.GetService<IJsonSerializer>();
            var jsonData = jsonSerializer.Serialize(postItem);

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
