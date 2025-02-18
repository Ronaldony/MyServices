using LiteNetLib;
using LiteNetLib.Utils;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var listener = new EventBasedNetListener();
            var client = new NetManager(listener);
            client.UnsyncedEvents = true;

            var sendString = string.Empty;

            client.Start();
            client.Connect("localhost" , 9050, "SomeConnectionKey");
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                Console.WriteLine("We got: {0}", dataReader.GetString());
				dataReader.Recycle();
            };

            listener.PeerConnectedEvent += (peer) =>
			{
                Console.WriteLine($"Connected to server.");
				// Send to server.
				var writer = new NetDataWriter();         // Create writer class
				var guid = Guid.NewGuid();

				writer.Put($"To server. Guid: {guid}");
				//peer.Send(writer, DeliveryMethod.ReliableOrdered);
			};

            while (true)
            {
                //client.PollEvents();
                sendString = Console.ReadLine();

                var writer = new NetDataWriter();
                writer.Put(sendString);

                client.FirstPeer.Send(writer, DeliveryMethod.ReliableOrdered);

				Thread.Sleep(15);
            }

            client.Stop();
        }
    }
}