using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Externals.UDP
{
    /// <summary>
    /// UdpServer.
    /// </summary>
    public abstract class UdpClient : Singleton<UdpClient>
    {
        // Network.
        private NetPacketProcessor _packetProcessor;
        private EventBasedNetListener _listener;
        private NetManager _netManager;
        public NetPeer _peer;
        public NetDataWriter _dataWriter;
        
        // Server address.
        private readonly string _addr;

        /// <summary>
        /// UDP handler dictionary.
        /// key: packet code.
        /// value: UDP handler.
        /// </summary>
        private Dictionary<int, (UdpHandler handler, MethodInfo method, object[] args)> _handlerDict;

        public UdpClient()
        {
            // Initialize network.
            _packetProcessor = new NetPacketProcessor();
            _listener = new EventBasedNetListener();

            // Encryption layer.
            var xorEncryptLayer = new XorEncryptLayer("ProjectRPG");

            _netManager = new NetManager(_listener, xorEncryptLayer)
            {
                ReuseAddress = false,               // Reuse address.
                UnsyncedEvents = true,              // Event base network.
                AutoRecycle = true,                 // Auto NetPacketReader Recycle.
                SimulateLatency = false,
                SimulatePacketLoss = false,
                EnableStatistics = true,
            };

            _dataWriter = new NetDataWriter();
            _packetProcessor = new NetPacketProcessor();

            // Handler.
            _handlerDict = new Dictionary<int, (UdpHandler, MethodInfo, object[])>();

            // Initialize listener.
            Initialize_Listener();

            // Initialize handler.
            Initialize_Handlers();

            // Register UDP packet.
            _packetProcessor.SubscribeReusable<UdpPacket>(HandlePacket);

            // Start server.
            //_netManager.AllowPeerAddressChange = allowPeerAddrChange;
        }

        /// <summary>
        /// Connect.
        /// </summary>
        public void Start(string server, int port)
        {
            _netManager.Start();
            _netManager.Connect(server, port, "");
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(UdpPacket packet)
        {
            if (false == _peer.ConnectionState.Equals(ConnectionState.Connected))
            {
                return false;
            }

            // Write packet.
            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);

            // Send.
            _peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);

            return true;
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        public bool BroadCast(UdpPacket packet)
        {
            // Sesison lock: 세션 풀을 사용하기 때문에, 이미 Disconnection된 세션 메모리가 사용되는 것을 방지.
            if (false == _peer.ConnectionState.Equals(ConnectionState.Connected))
            {
                return false;
            }

            // Write packet.
            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);

            // Send all.
            _netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered, _peer);

            return true;
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect()
        {
            _peer.Disconnect();
        }

        /// <summary>
        /// Initialize listener.
        /// </summary>
        private void Initialize_Listener()
        {
            // OnConnected.
            _listener.PeerConnectedEvent += (peer) =>
            {
                _peer = peer;
                _dataWriter.Reset();

                // Register packet processor.
                _packetProcessor = new NetPacketProcessor();
                _packetProcessor.SubscribeReusable<UdpPacket>(HandlePacket);
            };

            // OnReceive.
            _listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
            {
                // Read all packets.
                _packetProcessor.ReadAllPackets(reader);
            };

            // OnNetError.
            _listener.NetworkErrorEvent += (endPoint, socketError) =>
            {
            };

            // OnDisconnected.
            _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                _peer = peer;
            };
        }

        /// <summary>
        /// Initialize handlers.
        /// </summary>
        private void Initialize_Handlers()
        {
            var handlerTypes = Assembly.GetEntryAssembly().GetTypes().Where(d => d.IsSubclassOf(typeof(UdpHandler)));

            _handlerDict.Clear();

            foreach (var type in handlerTypes)
            {
                var handler = Activator.CreateInstance(type) as UdpHandler;
                var method = handler.GetType().GetMethod(nameof(UdpHandler.HandlePacket));

                _handlerDict.Add(handler.PacketCode, (handler, method, new object[1]));
            }
        }

        /// <summary>
        /// Handle udp packet.
        /// </summary>
        private void HandlePacket(UdpPacket packet)
        {
            // Handle udp packet.
            if (_handlerDict.TryGetValue(packet.PacketCode, out var value))
            {
                value.method.Invoke(value.handler, new object[1] { packet });
            }
        }
    }
}
