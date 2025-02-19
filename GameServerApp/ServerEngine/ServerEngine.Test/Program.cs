using PowerArgs;
using System.Net;
using System.Net.Sockets;

namespace ServerEngine.Test
{
	using LiteNetLib;
	using LiteNetLib.Utils;
	using ServerEngine.AsciiBanner;

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

            ThreadPool.QueueUserWorkItem(TestCode);

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

        /// <summary>
        /// Test code.
        /// </summary>
        private static void TestCode(object param)
        {
            var port = 9050;
            var listener = new EventBasedNetListener();
            var server = new NetManager(listener);

            server.UnsyncedEvents = true;
			server.Start(port);

            // Request connection
			listener.ConnectionRequestEvent += request =>
            {
                //request.AcceptIfKey("SomeConnectionKey");
                request.Accept();
				Console.WriteLine($"Peer count: {server.ConnectedPeersCount}");
			};

            // Connected
            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer);  // Show peer ip
                var writer = new NetDataWriter();         // Create writer class
                writer.Put("Hello client!");                        // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability
            };

            // Receive packet
            listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
            {
                var readString = reader.GetString();
				Console.WriteLine($"peer - Id: {peer}");
				Console.WriteLine($"Send string - {readString}");

				var writer = new NetDataWriter();         // Create writer class
				writer.Put(readString);                        // Put some string
                server.SendToAll(writer, DeliveryMethod.ReliableOrdered, peer);
            };

            // Disconnected
            listener.PeerDisconnectedEvent += (NetPeer peer, DisconnectInfo disconnectInfo) =>
            {
				Console.WriteLine($"disconnected - {disconnectInfo.Reason} / {peer} / {peer.Id}");
			};

            // Network error.
            listener.NetworkErrorEvent += (IPEndPoint endPoint, SocketError socketError) =>
            {
				Console.WriteLine($"Error event - {endPoint} / {socketError}");
            };

			while (!Console.KeyAvailable)
            {
                //server.PollEvents();
                Thread.Sleep(1000000000);
            }

            server.Stop();
        }
    }
}