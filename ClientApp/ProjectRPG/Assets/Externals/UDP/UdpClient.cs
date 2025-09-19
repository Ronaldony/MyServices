using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using ServerEngine.Network.UDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Externals.UDP
{
    /// <summary>
    /// UdpServer.
    /// </summary>
    public class UdpClient
    {
        // Network.
        private GamePacketProcessor _packetProcessor;
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
            _packetProcessor = new GamePacketProcessor();
            _listener = new EventBasedNetListener();

            // Encryption layer.
            var xorEncryptLayer = new XorEncryptLayer("Project_RPG");

            _netManager = new NetManager(_listener, xorEncryptLayer)
            {
                ReuseAddress = false,               // Reuse address.
                UnsyncedEvents = true,              // Event base network.
                AutoRecycle = true,                 // Auto GamePacketReader Recycle.
                SimulateLatency = false,
                SimulatePacketLoss = false,
                EnableStatistics = true,
            };

            _dataWriter = new NetDataWriter();
            _packetProcessor = new GamePacketProcessor();

            // Handler.
            _handlerDict = new Dictionary<int, (UdpHandler, MethodInfo, object[])>();

            // Initialize listener.
            Initialize_Listener();

            // Register UDP packet.
            _packetProcessor.SubscribeReusable<UdpPacket>(HandlePacket);
        }

        /// <summary>
        /// Initialize handlers.
        /// </summary>
        public void Initialize_Handlers<T>() where T : UdpHandler
        {
            _handlerDict.Clear();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes().Where(d => d.IsSubclassOf(typeof(T))))
                {
                    var handler = Activator.CreateInstance(type) as T;
                    var method = handler.GetType().GetMethod(nameof(handler.HandlePacket));

                    _handlerDict.Add(handler.PacketCode, (handler, method, new object[1]));
                }
            }
        }

        /// <summary>
        /// Connect.
        /// </summary>
        public bool Connect(string server, int port)
        {
            _netManager.Start();
            
            _peer = _netManager.Connect(server, port, "");

            if (_peer == null)
            {
                return false;
            }

            return true;
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
            // Sesison lock: ���� Ǯ�� ����ϱ� ������, �̹� Disconnection�� ���� �޸𸮰� ���Ǵ� ���� ����.
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
                _packetProcessor = new GamePacketProcessor();
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

        /// <summary>
        /// Get ping (ms)
        /// </summary>
        public int GetPing()
        {
            if (null == _peer)
            {
                return 0;
            }

            return _peer.Ping;
        }
    }
}
