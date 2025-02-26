using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ServerEngine.Network.UDP
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.Interfaces;

    /// <summary>
    /// UdpServer.
    /// </summary>
    public abstract class UdpServer<TSession> : IUdpServer
        where TSession : UdpSession, new()
    {
        private readonly ILogger<UdpServer<TSession>> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly IDataSerializer _serializer;

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
        private Dictionary<int, (UdpHandler handler, MethodInfo method, object[] args)> _handlerDict;

        /// <summary>
        /// Session dictonary.
        /// key: session id.
        /// value: UDP Session interface.
        /// </summary>
        private readonly Dictionary<int, TSession> _sessionDict;

        /// <summary>
        /// Session uid.
        /// </summary>
        private ulong _sessionUid;

        // Call back Delegates for network listener.
        protected abstract void OnConnected(ref TSession session, IPAddress ipAddr);
        protected abstract void OnNetError(SocketError sockError);
        protected abstract void OnDisconnected(TSession session);

		public UdpServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<UdpServer<TSession>>>();

            _memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            _serializer = _serviceProvider.GetRequiredService<IDataSerializer>();

            // Initialize network.
            _packetProcessor = new NetPacketProcessor();
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener)
            {
                ReuseAddress = false,               // Reuse address.
                UnsyncedEvents = true,              // Event base network.
                AutoRecycle = true,                 // Auto NetPacketReader Recycle.
                //UnsyncedReceiveEvent = true,        // Receive event will be called own thread.
                PacketPoolSize = 1000000,
            };

            // Handler.
            _handlerDict = new Dictionary<int, (UdpHandler, MethodInfo, object[])>();
            _sessionDict = new Dictionary<int, TSession>();

            // Black list.
            _whiteList = new List<IPAddress>();
        }

		/// <summary>
		/// Start server.
		/// </summary>
		/// <param name="port">Server port</param>
		/// <param name="allowPeerAddrChange">Allow peer address changable (lte to wifi, wifi to let etc).</param>
		/// <param name="pollingMilliseconds">Polling event time. Default time: 33ms (about 30 fps) </param>
		public void Start(int port, bool allowPeerAddrChange)
        {
			// Initialize listener.
			Initialize_Listener();

            // Initialize handler.
            Initialize_Handlers();

            // Register UDP packet.
            _packetProcessor.SubscribeReusable<UdpPacket, TSession>(HandlePacket);

            // Start server.
            _netManager.AllowPeerAddressChange = allowPeerAddrChange;
            _netManager.Start(port);
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            // Sesison lock: 세션 풀을 사용하기 때문에, 이미 Disconnection된 세션 메모리가 사용되는 것을 방지.
            lock (session)
            {
                // Check disconnection.
                if (session.IsDisconn)
                {
                    return false;
                }

                // Check session uid.
                if (sessionUid != session.SessionUid)
                {
                    return false;
                }

                // Write packet.
                var writer = session.DataWriter;

                writer.Reset();
                //_packetProcessor.Write(writer, packet);
                session.PacketProcessor.Write(writer, packet);

                // Send.
                session.SendCount++;
                session.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }

            return true;
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        public bool BroadCast(int sessionId, ulong sessionUid, UdpPacket packet)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            // Sesison lock: 세션 풀을 사용하기 때문에, 이미 Disconnection된 세션 메모리가 사용되는 것을 방지.
            lock(session)
            {
                // Check disconnection.
                if (session.IsDisconn)
                {
                    return false;
                }

                // Check sesion uid.
                if (session.SessionUid != sessionUid)
                {
                    return false;
                }

                // Write packet.
                var writer = session.DataWriter;

                writer.Reset();
                //_packetProcessor.Write(writer, packet);
                session.PacketProcessor.Write(writer, packet);

                // Send all.
                _netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, session.Peer);

                return true;
            }
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect(int sessionId, ulong sessionUid)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return;
            }

            // Sesison lock: 세션 풀을 사용하기 때문에, 이미 Disconnection된 세션 메모리가 사용되는 것을 방지.
            lock (session)
            {
                // Check connection.
                if (session.IsDisconn)
                {
                    return;
                }

                // Check session uid.
                if (session.SessionUid != sessionUid)
                {
                    return;
                }

                session.Peer.Disconnect();
            }
        }

        /// <summary>
        /// Add white list.
        /// </summary>
        public bool AddWhiteList(string addressString)
        {
            if (false == IPAddress.TryParse(addressString, out var ipAddr))
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
				// Check white list.
				if (_whiteList.Any())
				{
					// Disconnect if not in white list.
					if (default == _whiteList.FirstOrDefault(d => d.Equals(request.RemoteEndPoint.Address)))
					{
						_logger.LogError($"[UDP request connection] Session not found. id: {request.RemoteEndPoint.Address}");
						return;
					}
				}

                request.Accept();
			};

            // OnConnected.
            _listener.PeerConnectedEvent += (peer) =>
			{
                var newSession = _memoryPoolService.Rent<TSession>();

                lock (newSession)
                {
                    newSession.SessionId = peer.Id;
                    newSession.SessionUid = _sessionUid++;
                    newSession.IsDisconn = false;
                    newSession.Peer = peer;
                    newSession.Address = peer.Address;
                    newSession.DataWriter = newSession.DataWriter ??= new NetDataWriter();
                    newSession.DataWriter.Reset();

                    // Register packet processor.
                    if (null == newSession.PacketProcessor)
                    {
                        newSession.PacketProcessor = new NetPacketProcessor();
                        newSession.PacketProcessor.SubscribeReusable<UdpPacket, TSession>(HandlePacket);
                    }
                }

                // Check session id duplicate.
                _sessionDict.Add(newSession.SessionId, newSession);

                OnConnected(ref newSession, newSession.Address);
			};

			// OnReceive.
			_listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
			{
                var session = _sessionDict[peer.Id];

                session.RecvCount++;

                // Read all packets.
                session.PacketProcessor.ReadAllPackets(reader, session);
            };

			// OnNetError.
			_listener.NetworkErrorEvent += (endPoint, socketError) =>
			{
				OnNetError(socketError);
			};

			// OnDisconnected.
			_listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
			{
                var session = _sessionDict[peer.Id];

				OnDisconnected(session);

                // Remove session.
                //_sessionDict.Remove(peer.Id);

                _logger.LogDebug("[UDP disconnect] Session id: {0, -3}, Send: {1, -5}, Recv: {2, -5}, HandleCount: {3, -5}", session.SessionId, session.SendCount, session.RecvCount, session.HandleCount);

                lock (session)
                {
                    // Release session memory.
                    session.Peer = null;
                    session.Address = null;
                    session.IsDisconn = true;

                    _memoryPoolService.Release(session);
                }
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
                var method = handler.GetType().GetMethod(nameof(UdpHandler.HandlePacket));

                _handlerDict.Add(handler.PacketCode, (handler, method, new object[3]));
            }
        }
        
        /// <summary>
        /// Handle udp packet.
        /// </summary>
		private void HandlePacket(UdpPacket packet, TSession session)
        {
            session.HandleCount++;

            // Handle udp packet.
            if (_handlerDict.TryGetValue(packet.PacketCode, out var value))
            {
                var send = this.Send;
                var args = new object[3];

                args[0] = new UdpPacket
                {
                    PacketCode = packet.PacketCode,
                    Data = packet.Data,
                };

                args[1] = session;
                args[2] = send;

                // Invoke method.
                ThreadPool.QueueUserWorkItem(param =>
                {
                    value.method.Invoke(value.handler, args);
                });
            }
        }
    }
}
