using LiteNetLib;
using LiteNetLib.Utils;
using System.Text.Json;

namespace TestClientApp.Test
{
    using LiteNetLib.Layers;
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

            public int SendCount { get; set; }

            public int RecvCount { get; set; }

            public bool IsTesting { get; set; }
            
            public int SessionId { get; set; }
        }

        private const int TestPacketCode = 1000;

        private Dictionary<int, NetworkTestGroup> _testGroupDict;

        private readonly MemoryPackDataSerializer _serializer;

        private readonly int _port;

        private readonly int _maxTestCount;

        public Test_Network_DataIntegrity(int clientNum, int port, int maxTestCount)
        {
			_maxTestCount = maxTestCount;
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
                    SessionId = sessionId
                };

                // Client.
                var xorEncryptLayer = new XorEncryptLayer("ProjectRPG");

                testGroup.Client = new NetManager(testGroup.Listener, xorEncryptLayer);
                testGroup.Client.UnsyncedEvents = true;
                testGroup.Client.AutoRecycle = true;
                testGroup.Client.ReuseAddress = false;
				testGroup.Client.SimulatePacketLoss = false;
				testGroup.Client.EnableStatistics = true;

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
                    if (dataReader.RawData.Length < 93)
                    {
                        CustomLogger.Log($"Length: {dataReader.RawData.Length}, session id: {sessionId}");
                    }

                    testGroup.Processor.ReadAllPackets(dataReader, sessionId);
                };

                testGroup.Listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
                {
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

                if (group.Client.ConnectedPeersCount != 0 )
                {
                    group.Client.DisconnectAll();
                }
            }

            // Connection.
            foreach (var pair in _testGroupDict)
            {
                var group = pair.Value;

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

                    ThreadPool.QueueUserWorkItem(parma =>
					{
                        Thread.Sleep(100);

						group.RecvCount++;

						if (group.RecvCount % 1000 == 0)
						{
							//CustomLogger.Log($"[Testing] session id: {group.SessionId}, test count: {group.RecvCount}");
						}

						// 최대 테스트 시 클라이언트 종료.
						if (group.RecvCount < _maxTestCount)
						{
							// Send packet.
							var writer = new NetDataWriter();

							group.SendCount++;
							group.Processor.Write(writer, group.Packet);
							group.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
						}
						else
						{
							// 테스트 종료.
							group.RecvCount = 0;
							group.IsTesting = false;
							group.Client.DisconnectAll();
							CustomLogger.Log($"[Test end] session id: {group.SessionId}");
						}
					});
				}
                else
                {
                    CustomLogger.Log($"Not exist session. session id: {sessionId}");
                }
            }
            else
            {
                CustomLogger.Log($"Packet code is wrong. recv code: {packet.PacketCode}");
			}
		}
    }
}
