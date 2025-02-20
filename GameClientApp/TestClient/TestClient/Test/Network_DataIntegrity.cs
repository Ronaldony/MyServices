using LiteNetLib;
using LiteNetLib.Utils;

namespace TestClient.Test
{
    using ServerEngine.Core.Services;
    using ServerEngine.Network.UDP;
    using System.Text.Json;
    using TestClient.Test.Data;

    /// <summary>
    /// Network_DataIntegrity.
    /// Description: Test class to check integrity for network data.
    /// </summary>
    public class Network_DataIntegrity
    {
        /// <summary>
        /// NetworkGroup.
        /// </summary>
        public class NetworkGroup
        {
            public EventBasedNetListener Listener { get; set; }

            public NetManager Client { get; set; }

            public UdpPacket Packet { get; set; }

            public NetPeer Peer { get; set; }

            public NetPacketProcessor Processor { get; set; }
        }

        private const int TestPacketCode = 1000;

        private Dictionary<int, NetworkGroup> _networkGroup;

        private readonly MemoryPackDataSerializer _serializer;

        public Network_DataIntegrity(int clientNum, int port)
        {
            _serializer = new MemoryPackDataSerializer();

            _networkGroup = new Dictionary<int, NetworkGroup>();

            for (int i = 0; i < clientNum; i++)
            {
                var sessionId = i;
                var processor = new NetPacketProcessor();
                var listener = new EventBasedNetListener();
                var client = new NetManager(listener);
                var sendPacket = new UdpPacket();

                sendPacket.PacketCode = TestPacketCode;
                sendPacket.Data = _serializer.Serialize(new Network_TestData
                {
                    Birth = DateTime.Now.AddDays(i),
                    Height = i,
                    IsMarried = false,
                    Name = Guid.NewGuid().ToString(),
                    Years = Random.Shared.Next(),
                });

                var group = new NetworkGroup
                {
                    Listener = listener,
                    Client = client,
                    Packet = sendPacket,
                    Processor = processor
                };

                // netrwork.
                client.UnsyncedEvents = true;
                processor.SubscribeReusable<UdpPacket, int>(HandlePacket);

                // Listener.
                listener.PeerConnectedEvent += (peer) =>
                {
                    group.Peer = peer;

                    Console.WriteLine($"Connected to server. cleint {sessionId}");
                    
                    var writer = new NetDataWriter();
                    processor.Write(writer, sendPacket);

                    // Send to server.
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                };

                listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
                {
                    processor.ReadAllPackets(dataReader, sessionId);

                    dataReader.Recycle();
                };

                group.Client.Start();
                group.Client.Connect("localhost", port, "");

                _networkGroup.Add(i, group);
            }
        }

        /// <summary>
        /// Test.
        /// </summary>
        public void Test()
        {
            while(true)
            {
                Thread.Sleep(5000);
                Console.WriteLine($"Pass 5 seconds.");
            }
        }

        private void HandlePacket(UdpPacket packet, int sessionId)
        {
            if (packet.PacketCode == TestPacketCode)
            {
                if (_networkGroup.TryGetValue(sessionId, out var group))
                {
                    var oriData = _serializer.Deserialize<Network_TestData>(group.Packet.Data);
                    var recvData = _serializer.Deserialize<Network_TestData>(packet.Data);

                    var oriJson = JsonSerializer.Serialize(oriData);
                    var recvJson = JsonSerializer.Serialize(recvData);

                    if (false == oriJson.Equals(recvJson))
                    {
                        Console.WriteLine($"Pakcet not matching against original. session id: {sessionId}");
                    }

                    var writer = new NetDataWriter();
                    group.Processor.Write(writer, group.Packet);

                    // Send
                    group.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
                else
                {
                    Console.WriteLine($"Not exist session. session id: {sessionId}");
                }
            }
        }
    }
}
