using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ServerEngine.Network.UDP
{
    using ServerEngine.Network.Interfaces;

    /// <summary>
    /// UdpServer.
    /// </summary>
    public abstract class UdpServer<TSession> : IUdpServer<TSession>
        where TSession : UdpSession
    {
        private readonly ILogger<UdpServer<TSession>> _logger;

        private readonly IServiceProvider _serviceProvider;

        // Network.
        private NetPacketProcessor _packetProcessor;
        private EventBasedNetListener _listener;
        private NetManager _netManager;

        /// <summary>
        /// White list.
        /// </summary>
        private readonly List<IPAddress> _whiteList;

        /// <summary>
        /// UDP handler dictionary.
        /// key: packet code.
        /// value: UDP handler.
        /// </summary>
        private Dictionary<int, (UdpHandler, MethodInfo)> _handlerDict;

        /// <summary>
        /// Session dictonary.
        /// key: session id.
        /// value: UDP Session interface.
        /// </summary>
        private readonly ConcurrentDictionary<int, TSession> _sessionDict;

        // Call back Delegates for network listener.
        protected abstract void OnRequestConnection();
        protected abstract TSession OnConnected(int sessionId, IPAddress ipAddr);
        protected abstract void OnNetError(SocketError sockError);
        protected abstract void OnDisconnected(TSession session);

		public UdpServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<UdpServer<TSession>>>();

            // Initialize network.
            _packetProcessor = new NetPacketProcessor();
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener)
            {
                BroadcastReceiveEnabled = true,     // Broadcast
                ReuseAddress = false,               // Reuse address.
                UnsyncedEvents = true,              // Event base network.
            };

            // Handler.
            _handlerDict = new Dictionary<int, (UdpHandler, MethodInfo)>();
            _sessionDict = new ConcurrentDictionary<int, TSession>();

            // Black list.
            _whiteList = new List<IPAddress>();
        }

        /// <summary>
        /// Start server.
        /// </summary>
        public void Start(int port)
        {
			// Initialize listener.
			Initialize_Listener();

            // Initialize handler.
            Initialize_Handlers();

            // Register UDP packet.
            _packetProcessor.SubscribeReusable<UdpPacket, TSession>(HandlePacket);

            // Start server.
            ThreadPool.QueueUserWorkItem((param) =>
            {
                _netManager.Start(port);
            });
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(int sessionId, UdpPacket packet)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            // Write packet.
            var writer = session.DataWriter;

            writer.Reset();
            _packetProcessor.Write(writer, packet);

            // Send.
            session.Peer.Send(writer, DeliveryMethod.ReliableOrdered);

            return true;
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        public bool BroadCast(int sessionId, UdpPacket packet)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            // Write packet.
            var writer = session.DataWriter;

            writer.Reset();
            _packetProcessor.Write(writer, packet);

            // Send.
            _netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, session.Peer);

            return true;
        }

        /// <summary>
        /// Get session.
        /// </summary>
        public TSession GetSessionOrDefault(int sessionId)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return default;
            }

            return session;
        }

        /// <summary>
        /// Add white list.
        /// </summary>
        public bool AddWhiteList(string addressString)
        {
            IPAddress ipAddr;

            if (false == IPAddress.TryParse(addressString, out ipAddr))
            {
                return false;
            }

            _whiteList.Add(ipAddr);

            return true;
        }

        /// <summary>
        /// Initialize listener.
        /// </summary>
        private void Initialize_Listener()
		{
			// OnRequestConnection.
			_listener.ConnectionRequestEvent += (request) =>
			{
				request.Accept();
				OnRequestConnection();
			};

			// OnConnected.
			_listener.PeerConnectedEvent += (peer) =>
			{
				// Check white list.
				if (_whiteList.Any())
				{
					// Disconnect if not in white list.
					if (default == _whiteList.FirstOrDefault(d => d.Equals(peer.Address)))
                    {
                        _logger.LogError($"[UDP Receive] Session not found. id: {peer.Id} / peer: {peer}");
                        peer.Disconnect();
                        return;
					}
				}

                // Add session.
                // OnConnected.
                var session = OnConnected(peer.Id, peer.Address);
                if (null == session)
                {
                    _logger.LogError($"[UDP Receive] Session not created. id: {peer.Id} / peer: {peer}");
                    peer.Disconnect();
                    return;
                }

                session.Peer = peer;
                session.DataWriter = new NetDataWriter();

                // Check session id duplicate.
                _sessionDict.AddOrUpdate(peer.Id, session, (key, old) => session);
			};

			// OnReceive.
			_listener.NetworkReceiveEvent += (NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) =>
			{
				if (false == _sessionDict.TryGetValue(peer.Id, out var session))
				{
					_logger.LogError($"[UDP Receive] Session not found. id: {peer.Id} / peer: {peer}");
					peer.Disconnect();
					return;
				}

                // Read all packets.
                _packetProcessor.ReadAllPackets(reader, session);
			};

			// OnNetError.
			_listener.NetworkErrorEvent += (endPoint, socketError) =>
			{
				OnNetError(socketError);
			};

			// OnDisconnected.
			_listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
			{
                if (false == _sessionDict.TryGetValue(peer.Id, out var session))
                {
                    _logger.LogError($"[UDP Disconnect] not found. id: {peer.Id} / peer: {peer}");
                    peer.Disconnect();
                    return;
                }

				OnDisconnected(session);

                // Remove session.
                _sessionDict.Remove(peer.Id, out _);
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
                var handler = Activator.CreateInstance(type, _serviceProvider) as UdpHandler;
                var baseMethod = handler.GetType().GetMethod(nameof(UdpHandler.HandlePacket));
                var method = baseMethod.MakeGenericMethod(typeof(TSession));

                _handlerDict.Add(handler.PacketCode, (handler, method));
            }
        }
        
        /// <summary>
        /// Handle udp packet.
        /// </summary>
		private void HandlePacket(UdpPacket packet, TSession session)
        {
            // Handle udp packet.
            if (_handlerDict.TryGetValue(packet.PacketCode, out var value))
            {
                var handler = value.Item1;
                var method = value.Item2;

                // Invoke method.
                method?.Invoke(handler, new object [] { packet, session } );
            }
        }
    }
}
