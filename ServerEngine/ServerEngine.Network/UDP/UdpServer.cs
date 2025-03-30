using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Akka.Actor;

namespace ServerEngine.Network.UDP
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.Actors.Udp;
    using ServerEngine.Network.Interfaces;

    /// <summary>
    /// UdpServer.
    /// </summary>
    public abstract class UdpServer : IUdpServer
    {
        private readonly ILogger<UdpServer> _logger;

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
        private readonly Dictionary<int, UdpSession> _sessionDict;

        /// <summary>
        /// UdpSendActorProvider dictionary.
        /// key: hash for session id.
        /// value: UdpSendActorProvider.
        /// </summary>
        private readonly Dictionary<int, UdpSendActorProvider> _udpSendActorDict;

        /// <summary>
        /// Session uid.
        /// </summary>
        private ulong _sessionUid;
        
        // Call back Delegates for network listener.
        protected abstract void OnConnected(int sessionId, ulong sessionUid);
		protected abstract void OnNetError(SocketError sockError);
        protected abstract void OnDisconnected(int sessionId, ulong sessionUid);

		public UdpServer(IServiceProvider serviceProvider, ActorSystem actorSystem)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<UdpServer>>();

            _memoryPoolService = _serviceProvider.GetRequiredService<IMemoryPoolService>();
            _serializer = _serviceProvider.GetRequiredService<IDataSerializer>();

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

            // Handler.
            _handlerDict = new Dictionary<int, (UdpHandler, MethodInfo, object[])>();
            _sessionDict = new Dictionary<int, UdpSession>();

            // Black list.
            _whiteList = new List<IPAddress>();

            // Initialize udp actor.
            _udpSendActorDict = Initialize_UpdSendActor(_serviceProvider, actorSystem, _netManager);
        }

        /// <summary>
        /// Initialize udp send actors.
        /// </summary>
        private Dictionary<int, UdpSendActorProvider> Initialize_UpdSendActor(IServiceProvider serviceProvider, ActorSystem actorSystem, NetManager netManager)
        {
            // 논리 프로세서 개수의 1/4 만큼 액터 색성.
            var actorCount = Environment.ProcessorCount / 2;

            if (actorCount == 0)
            {
                actorCount = 1;
            }

            // 액터 생성.
            var actorDict = new Dictionary<int, UdpSendActorProvider>();

            for (var cnt = 0; cnt < actorCount; ++cnt)
            {
                actorDict.Add(cnt, new UdpSendActorProvider(actorSystem.ActorOf(UdpSendActor.Props(serviceProvider, netManager))));
            }

            return actorDict;
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
            _packetProcessor.SubscribeReusable<UdpPacket, UdpSession>(HandlePacket);

            // Start server.
            _netManager.AllowPeerAddressChange = allowPeerAddrChange;
            _netManager.Start(port);
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool Send(int sessionId, ulong sessionUid, int packetCode, byte[] packetData)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            // 핑퐁 테스트 간 전송 직전 최소한의 딜레이를 주어
            // 네트워크 라이브러리 네트워크 버퍼에서 발생할 수 있는 Race condition 문제를 없앰.
            //Thread.Sleep(5);

            // 세션 Id에 해당하는 UdpSendActor에 메시지 전달.
            var actorMsg = _memoryPoolService.Rent<UdpSendActorMsg_Send>();

            actorMsg.Session = session;
            actorMsg.SessionId = sessionId;
            actorMsg.SessionUid = sessionUid;
            actorMsg.PacketCode = packetCode;
            actorMsg.PacketData = packetData;

            _udpSendActorDict[sessionId % _udpSendActorDict.Count].Tell<UdpSendActorMsg_Send>(actorMsg);

            return true;
        }

        /// <summary>
        /// Broadcast packet.
        /// </summary>
        public bool BroadCast(int sessionId, ulong sessionUid, int packetCode, byte[] packetData)
        {
            if (false == _sessionDict.TryGetValue(sessionId, out var session))
            {
                return false;
            }

            // 핑퐁 테스트 간 전송 직전 최소한의 딜레이를 주어
            // 네트워크 라이브러리 네트워크 버퍼에서 발생할 수 있는 Race condition 문제를 없앰.
            //Thread.Sleep(5);

            // 세션 Id에 해당하는 UdpSendActor에 메시지 전달.
            var actorMsg = _memoryPoolService.Rent<UdpSendActorMsg_Broadcast>();

            actorMsg.Session = session;
            actorMsg.SessionId = sessionId;
            actorMsg.SessionUid = sessionUid;
            actorMsg.PacketCode = packetCode;
            actorMsg.PacketData = packetData;

            _udpSendActorDict[sessionId % _udpSendActorDict.Count].Tell<UdpSendActorMsg_Broadcast>(actorMsg);

            return true;
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
                var newSession = _memoryPoolService.Rent<UdpSession>();

                lock (newSession)
                {
                    newSession.SessionId = peer.Id;
                    newSession.SessionUid = _sessionUid++;
                    newSession.IsDisconn = false;
                    newSession.Peer = peer;
                    newSession.Address = peer.Address;
                    newSession.DataWriter ??= new NetDataWriter();
                    newSession.DataWriter.Reset();
                    newSession.Packet ??= new UdpPacket();

					// Register packet processor.
					if (null == newSession.PacketProcessor)
                    {
                        newSession.PacketProcessor = new NetPacketProcessor();
                        newSession.PacketProcessor.SubscribeReusable<UdpPacket, UdpSession>(HandlePacket);
                    }
                }

                // Check session id duplicate.
                _sessionDict.Add(newSession.SessionId, newSession);

                OnConnected(newSession.SessionId, newSession.SessionUid);
			};

			// OnReceive.
			_listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) =>
			{
                var session = _sessionDict[peer.Id];

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
				_sessionDict.Remove(session.SessionId);

				OnDisconnected(session.SessionId, session.SessionUid);

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
            _handlerDict.Clear();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes().Where(d => d.IsSubclassOf(typeof(UdpHandler))))
                {
                    var handler = Activator.CreateInstance(type, _serviceProvider) as UdpHandler;
                    var method = handler.GetType().GetMethod(nameof(UdpHandler.HandlePacket));

                    _handlerDict.Add(handler.PacketCode, (handler, method, new object[3]));
                }
            }
        }
        
        /// <summary>
        /// Handle udp packet.
        /// </summary>
		private void HandlePacket(UdpPacket packet, UdpSession session)
        {
            // Handle udp packet.
            if (_handlerDict.TryGetValue(packet.PacketCode, out var value))
            {
                var args = new object[3];

                // Set packet.
				session.Packet.PacketCode = packet.PacketCode;
				session.Packet.Data = packet.Data;

                // Set arguments. 
				args[0] = session.SessionId;
				args[1] = session.SessionUid;
                args[2] = session.Packet;

                // Invoke method.
                value.method.Invoke(value.handler, args);
            }
        }
    }
}
