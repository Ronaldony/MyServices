using LiteNetLib;
using LiteNetLib.Utils;
using ServerEngine.Core.Services;
using ServerEngine.Network.UDP;
using ServerEngine.Test.Database.Data;
using TestClient.Test;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var networkTestData = new Network_DataIntegrity(1000, 9050);
            networkTestData.Test();
        }

        public static void TestNetwork()
        {

            var listener = new EventBasedNetListener();
            var client = new NetManager(listener);
            client.UnsyncedEvents = true;

            var sendString = string.Empty;

            client.Start();
            client.Connect("localhost", 9050, "SomeConnectionKey");
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                //Console.WriteLine("We got: {0}", dataReader.GetString());
                dataReader.Recycle();
            };

            listener.PeerConnectedEvent += (peer) =>
            {
                Console.WriteLine($"Connected to server.");

                // PlayerInfo.
                var netSerilizer = new NetSerializer();
                var serilizer = new MemoryPackDataSerializer();
                var playerInfo = new DTO_PlayerInfo
                {
                    Pid = Guid.NewGuid().ToString(),
                    PlayerName = Guid.NewGuid().ToString(),
                    RegTime = DateTime.Now,
                    Uid = Guid.NewGuid().ToString(),
                };

                var packet = new UdpPacket
                {
                    PacketCode = 1000,
                    Data = serilizer.Serialize(playerInfo)
                };

                var writer = new NetDataWriter();
                var processor = new NetPacketProcessor();
                processor.Write(writer, packet);

                // Send to server.
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
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