using LiteNetLib;
using LiteNetLib.Utils;
using System.Text.Json;

namespace TestClientApp.Test
{
    using ServerEngine.Network.UDP;
    using TestClient.Test.Data;
    using TestClientApp.Services;

    /// <summary>
    /// NetworkTest_DataIntegrity.
    /// Description: Test class to check integrity for network data.
    /// </summary>
    public class Test_Network_DataIntegrity
    {
        /// <summary>
        /// NetworkGroup.
        /// </summary>
        public class NetworkTestGroup
        {
            public EventBasedNetListener Listener { get; set; }

            public NetManager Client { get; set; }

            public UdpPacket Packet { get; set; }

            public NetPeer Peer { get; set; }

            public NetPacketProcessor Processor { get; set; }

            public DateTime SendTime { get; set; }

            public double TotalRTT_MS { get; set; }

            public int MaxTestCount { get; set; }

            public int TestCount { get; set; }

            public int SendCount { get; set; }

            public int RecvCount { get; set; }

            public bool IsTesting { get; set; }
            
            public int SessionId { get; set; }
        }

        private const int TestPacketCode = 1000;

        private Dictionary<int, NetworkTestGroup> _testGroupDict;

        private readonly MemoryPackDataSerializer _serializer;

        private readonly int _port;

        private readonly int _clientNum;

        public Test_Network_DataIntegrity(int clientNum, int port, int maxTestCount)
        {
            _clientNum = clientNum;
            _testGroupDict = new Dictionary<int, NetworkTestGroup>();
            _serializer = new MemoryPackDataSerializer();
            _port = port;

            for (int i = 0; i < clientNum; i++)
            {
                var sessionId = i;
                var testGroup = new NetworkTestGroup
                {
                    Listener = new EventBasedNetListener(),
                    Packet = new UdpPacket(),
                    Processor = new NetPacketProcessor(),
                    MaxTestCount = maxTestCount,
                    SessionId = sessionId
                };

                // Client.
                testGroup.Client = new NetManager(testGroup.Listener);
                testGroup.Client.UnsyncedEvents = true;
                testGroup.Client.AutoRecycle = true;
                testGroup.Client.PacketPoolSize = 1000000;

                // Packet.
                testGroup.Packet.PacketCode = TestPacketCode;
                testGroup.Packet.Data = _serializer.Serialize(new Network_TestData
                {
                    Birth = DateTime.Now.AddDays(i),
                    Height = i,
                    IsMarried = false,
                    Name = Guid.NewGuid().ToString(),
                    Years = Random.Shared.Next(),
                });

                // Processor.
                testGroup.Processor.SubscribeReusable<UdpPacket, int>(HandlePacket);

                // Listener.
                testGroup.Listener.PeerConnectedEvent += (peer) =>
                {
                    testGroup.Peer = peer;

                    CustomLogger.Log($"Connected to server. cleint {sessionId}");
                    
                    var writer = new NetDataWriter();
                    testGroup.Processor.Write(writer, testGroup.Packet);

                    // Send to server.
                    testGroup.SendCount++;
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                };

                testGroup.Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
                {
                    testGroup.Processor.ReadAllPackets(dataReader, sessionId);
                };

                testGroup.Listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
                {
                    testGroup.TestCount = 0;
                    //testGroup.IsTesting = true;
                    //testGroup.Client.Connect("localhost", _port, "");

                    CustomLogger.Log($"Disconnected. session id: {sessionId}");
                };

                testGroup.Client.Start();

                _testGroupDict.Add(i, testGroup);
            }
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {
            // Disconnection.
            foreach (var pair in _testGroupDict)
            {
                var group = pair.Value;
                group.Client.DisconnectAll();
            }

            // Connection.
            foreach (var pair in _testGroupDict)
            {
                var group = pair.Value;

                group.SendTime = DateTime.Now;
                group.IsTesting = true;
                group.Client.Connect("localhost", _port, "");
            }
        }

        /// <summary>
        /// Check test is end.
        /// </summary>
        public bool IsTestEnd()
        {
            var testGroups = _testGroupDict.Values.Where(d => d.IsTesting == true);
            CustomLogger.Log($"Group count: {testGroups.Count()}");
            if (testGroups.Count() != 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// HandlePacket.
        /// </summary>
        private void HandlePacket(UdpPacket packet, int sessionId)
        {
            var recvTime = DateTime.Now;

            if (packet.PacketCode == TestPacketCode)
            {
                if (_testGroupDict.TryGetValue(sessionId, out var group))
                {
                    var oriData = _serializer.Deserialize<Network_TestData>(group.Packet.Data);
                    var recvData = _serializer.Deserialize<Network_TestData>(packet.Data);

                    var oriJson = JsonSerializer.Serialize(oriData);
                    var recvJson = JsonSerializer.Serialize(recvData);

                    // Error if original data is not matched with received data.
                    if (false == oriJson.Equals(recvJson))
                    {
                        CustomLogger.Log($"Pakcet not matching against original. session id: {sessionId}");
                        group.IsTesting = false;
                        return;
                    }

                    group.TestCount++;
                    group.RecvCount++;

                    var rtt = (recvTime - group.SendTime).TotalMilliseconds;
                    group.TotalRTT_MS += rtt;

                    if (group.TestCount % 500 == 0)
                    {
                        CustomLogger.Log($"[Testing] session id: {group.SessionId}, test count: {group.TestCount}");
                    }

                    // 최대 테스트 시 클라이언트 종료.
                    if (group.TestCount < group.MaxTestCount)
                    {
                        // Send packet.
                        var writer = new NetDataWriter();

                        group.SendTime = DateTime.Now;
                        group.SendCount++;
                        group.Processor.Write(writer, group.Packet);

                        Thread.Sleep(10);
                        group.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                    }
                    else
                    {
                        // 테스트 종료.
                        group.IsTesting = false;
                        //group.Peer.Disconnect();
                    }
                }
                else
                {
                    CustomLogger.Log($"Not exist session. session id: {sessionId}");
                }
            }
        }
    }
}
