using PowerArgs;

namespace ServerEngine.Test
{
    using LiteNetLib;
    using LiteNetLib.Utils;
    using ServerEngine.AsciiBanner;
    using System.Net;

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

            TestCode();

            var startup = new Startup(args, arguments);
            startup.Configure();

            var startupServer = new StartupServer(startup.WebApp.Services);
            await startupServer.ConfigureAsync();

            var asciiBannerWriter = new AsciiBannerWriter("slant.flf");
            Console.WriteLine(asciiBannerWriter.GetAsciiText(PROGRAM_NAME));

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
            var listener = new EventBasedNetListener();
            var server = new NetManager(listener);
            server.Start(9050 /* port */);

            listener.ConnectionRequestEvent += request =>
            {
                if (server.ConnectedPeersCount < 10 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer);  // Show peer ip
                var writer = new NetDataWriter();         // Create writer class
                writer.Put("Hello client!");                        // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
            };

            listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
            {
                Console.WriteLine($"peer - Id: {peer.Id}, MTU: {peer.Mtu}, Addr: {peer.Address}, Port: {peer.Port}, State: {peer.ConnectionState}");
                Console.WriteLine($"Channel - {channel}");
                Console.WriteLine($"Delivery - {deliveryMethod}");
                Console.WriteLine($"reader - Id: {reader.RawData}");
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }

            server.Stop();
        }
    }
}