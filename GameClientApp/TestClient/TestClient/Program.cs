using LiteNetLib;
using LiteNetLib.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
			MeasurePerformance();

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

        private static void MeasurePerformance()
        {
            int testTotal = 100000;
            
            var list = new List<AAA>();
            var listCopy = list;
			var hashSet = new HashSet<AAA>();
			var hashSetCopy = hashSet;

			// Stop watchs
			var sw = new Stopwatch();
			var hashSW = new Stopwatch();

			//////////////////////////////////////////////
			/// Add
			sw.Start();
            for (int cnt = 0; cnt < testTotal; cnt++)
            {
				list.Add(new AAA
                {
                    TestInt = cnt,
                    TestDouble = cnt,
                    TestDate = DateTime.Now,
                    TestBool = true,
                    TestString = cnt.ToString()
                });
			}
            sw.Stop();

			hashSW.Start();
			for (int cnt = 0; cnt < testTotal; cnt++)
			{
				hashSet.Add(new AAA
				{
					TestInt = cnt,
					TestDouble = cnt,
					TestDate = DateTime.Now,
					TestBool = true,
					TestString = cnt.ToString()
				});
			}
			hashSW.Stop();

			// Add
			Console.WriteLine($"List Add - ms: {sw.ElapsedMilliseconds}");
			Console.WriteLine($"Hash Add - ms: {hashSW.ElapsedMilliseconds}");

			sw.Reset();
			hashSW.Reset();

			/////////////////////////////////////////////////////////////
			/// Search.

			sw.Start();
            foreach (var item in listCopy)
            {
                if (default == list.Contains(item))
                {
                    Console.WriteLine($"Find list failed. {item.TestInt}");
                }
            }
            sw.Stop();


			hashSW.Start();
			foreach (var item in hashSetCopy)
			{
                if (default == hashSet.Contains(item))
				{
					Console.WriteLine($"Find hashSet failed. {item.TestInt}");
				}
			}
			hashSW.Stop();

			// Search
			Console.WriteLine($"List Search - ms: {sw.ElapsedMilliseconds}");
			Console.WriteLine($"Hash Search - ms: {hashSW.ElapsedMilliseconds}");

			sw.Reset();
			hashSW.Reset();

			/////////////////////////////////////////////////////////////
			/// Remove.

			sw.Start();
            for (int cnt = 0; cnt < testTotal; cnt++)
			{
				list.Remove(list.Last());
			}
			sw.Stop();

			hashSW.Start();
            for (int cnt = 0; cnt < testTotal; cnt++)
			{
				hashSet.Remove(hashSet.Last());
			}
			hashSW.Stop();

			// Search
			Console.WriteLine($"List Remove - ms: {sw.ElapsedMilliseconds}");
			Console.WriteLine($"Hash Remove - ms: {hashSW.ElapsedMilliseconds}");
		}
	}
}