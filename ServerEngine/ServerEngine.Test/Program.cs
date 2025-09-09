using PowerArgs;
using System.Reactive.Linq;

namespace ServerEngine.Test
{
    using ServerEngine.AsciiBanner;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.MessageBus.RabbitMQs;

    public class Conf_MessageBus
    {
        public Conf_MessageBus(string name, string exchange, string queue)
        {
            Name =  name;
            Exchange = exchange;
            Queue = queue;
        }

        public string Name;

        public string Exchange = "exchange";

        public string Queue;
    }

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

            var ss = new Dictionary<string, Conf_MessageBus>();

            ss.Add("playhub", new Conf_MessageBus("name", "exchange", "queue"));
            ss.Add("playhub2", new Conf_MessageBus("name", "exchange", "queue"));

            var jsonSer = startup.WebApp.Services.GetRequiredService<IJsonSerializer>();
            var dd = jsonSer.Serialize(ss);

            await TestCode(startup.WebApp.Services);

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
        private static async Task TestCode(IServiceProvider serviceProvider)
        {
            var dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            var rbmqConnection = new RBMQConnection(serviceProvider);

            if (rbmqConnection == null)
            {
                return;
            }

            await rbmqConnection.InitializeAsync("192.168.219.4", "project-rpg", "OiCxsHUSV44.Z]w3vc", new CancellationToken());

            // channel.
            var produceChannel = await rbmqConnection.CreateChannelAsync();

            if (null == produceChannel)
            {
                return;
            }

            // Exchange.
            await produceChannel.CreateExchangeAsync("exchange-test");

            // queue.
            var queue = await produceChannel.CreateQueueAsync("queue-test");

            // Bind queue.
            await produceChannel.BindQueueAsync(queue, "exchange-test", queue);

            // Produce.
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(async cnt =>
                {
                    const string message = "Hello World !";

                    Console.WriteLine($"count: {cnt}");
                    await produceChannel.PublishAsync("exchange-test", queue, message);
                });

            /////////////////////////////////////////////////////////
            /// Consumer.
            var consumeChannel = await rbmqConnection.CreateChannelAsync();

            await consumeChannel.RegisterConsumerAsync(queue, bytes =>
            {
                var message = dataSerializer.Deserialize<string>(bytes);

                Console.WriteLine($" Consumer 1 - [x] Received {message}");
            });

            var consumeChannel2 = await rbmqConnection.CreateChannelAsync();

            await consumeChannel2.RegisterConsumerAsync(queue, bytes =>
            {
                var message = dataSerializer.Deserialize<string>(bytes);

                Console.WriteLine($" Consumer 2 - [x] Received {message}");
            });
        }
    }
}