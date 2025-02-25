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
        private readonly IMemoryPoolService _poolService;

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
        private readonly ConcurrentDictionary<int, TSession> _sessionDict;

        /// <summary>
        /// Polling event time (milliseconds).
        /// </summary>
        private int _pollingEventTime;

        private ulong _sessionUid;

        // Call back Delegates for network listener.
        protected abstract void OnConnected(ref TSession session, IPAddress ipAddr);
        protected abstract void OnNetError(SocketError sockError);
        protected abstract void OnDisconnected(TSession session);

		public UdpServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<UdpServer<TSession>>>();

            _poolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();

			// Initialize network.
			_packetProcessor = new NetPacketProcessor();
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener)
            {
                ReuseAddress = false,               // Reuse address.
                //UnsyncedEvents = true,            // Event base network.
                AutoRecycle = true,                 // Auto NetPacketReader Recycle.
                UnsyncedReceiveEvent = true,        // Receive event will be called own thread.
            };

            // Handler.
            _handlerDict = new Dictionary<int, (UdpHandler, MethodInfo, object[])>();
            _sessionDict = new ConcurrentDictionary<int, TSession>();

            // Black list.
            _whiteList = new List<IPAddress>();
        }

		/// <summary>
		/// Start server.
		/// </summary>
		/// <param name="port">Server port</param>
		/// <param name="allowPeerAddrChange">Allow peer address changable (lte to wifi, wifi to let etc).</param>
		/// <param name="pollingMilliseconds">Polling event time. Default time: 33ms (about 30 fps) </param>
		public void Start(int port, bool allowPeerAddrChange, int pollingMilliseconds = 33)
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
            _pollingEventTime = pollingMilliseconds;

            // Polling events.
            ThreadPool.QueueUserWorkItem(param =>
            {
                while (true)
                {
                    _netManager.PollEvents();
                    Thread.Sleep(_pollingEventTime);
                }
            });
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(IUdpSession session, UdpPacket packet)
        {
			var findSession = GetSessionOrDefault(session.SessionId);
			if (default == findSession)
			{
				return false;
			}

			// 만약 세션 Uid가 다른 경우, 세션 ID가 같아도 이미 Disconnection된 세션임.
			if (!findSession.SessionUid.Equals(session.SessionUid))
			{
				return false;
			}

            var udpSession = findSession as UdpSession;

            // Write packet.
            var writer = udpSession.DataWriter;

            writer.Reset();
            _packetProcessor.Write(writer, packet);

            // Send.
            udpSession.Peer.Send(writer, DeliveryMethod.ReliableOrdered);

            return true;
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        public bool BroadCast(IUdpSession session, UdpPacket packet)
        {
            var findSession = GetSessionOrDefault(session.SessionId);
            if (default == findSession)
            {
                return false;
            }

            // 만약 세션 RefId 다른 경우, 세션 ID가 같아도 이미 Disconnection된 세션임.
            if (!findSession.SessionUid.Equals(session.SessionUid))
            {
                return false;
            }

            // Marshall session.
            var udpSession = findSession as UdpSession;

            // Write packet.
            var writer = udpSession.DataWriter;

            writer.Reset();
            _packetProcessor.Write(writer, packet);

            // Send all.
            _netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, udpSession.Peer);

            return true;
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public bool Disconnect(IUdpSession session)
        {
            // remove.
            var findSession = _sessionDict.GetValueOrDefault(session.SessionId);

            // 이미 Disconnection된 세션인지 검사.
            if (!session.Equals(findSession))
            {
                return false;
            }

            findSession.Peer.Disconnect();

            return true;
        }

        /// <summary>
        /// Get session.
        /// </summary>
        public IUdpSession GetSessionOrDefault(int sessionId)
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
            if (false == IPAddress.TryParse(addressString, out var ipAddr))
            {
                return false;
            }

            _whiteList.Add(ipAddr);

            return true;
		}

		/// <summary>
		/// Set polling interval time.
		/// </summary>
		public void SetPollingEventTime(int milliseconds)
		{
			_pollingEventTime = milliseconds;
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

                // Accept peer.
				var peer = request.Accept();

				if (null != peer)
                {
                    var newSession = _poolService.Rent<TSession>();

                    newSession.SessionId = peer.Id;
                    newSession.SessionUid = _sessionUid++;
					newSession.Peer = peer;
					newSession.Address = peer.Address;
					newSession.DataWriter = newSession.DataWriter ??= new NetDataWriter();
                    newSession.DataWriter.Reset();

                    // Check session id duplicate.
                    _sessionDict.AddOrUpdate(peer.Id, newSession, (key, old) => newSession);
                }
			};

            // OnConnected.
            _listener.PeerConnectedEvent += (peer) =>
			{
                var session = _sessionDict[peer.Id];
				
                OnConnected(ref session, session.Address);
			};

			// OnReceive.
			_listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
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
                var session = _sessionDict[peer.Id];

				OnDisconnected(session);

                _logger.LogDebug($"[UDP disconnect] Thread id: {Thread.CurrentThread.ManagedThreadId} / isPooled: {Thread.CurrentThread.IsThreadPoolThread}");
 
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
                var method = handler.GetType().GetMethod(nameof(UdpHandler.HandlePacket));

                _handlerDict.Add(handler.PacketCode, (handler, method, new object[3]));
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
                var send = this.Send;
                var args = value.args;

                args[0] = packet;
                args[1] = session;
                args[2] = send;

                // Invoke method.
                value.method?.Invoke(value.handler, args);
            }
        }
    }
}
