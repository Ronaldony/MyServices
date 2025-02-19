using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;

namespace ServerEngine.Network.UDP
{
	using ServerEngine.Network.Interfaces;

	/// <summary>
	/// UdpServer.
	/// </summary>
	public abstract class UdpServer
    {
        private readonly ILogger<UdpServer> _logger;

        // Network.
        private NetPacketProcessor _packetProcessor;
        private EventBasedNetListener _listener;
        private NetManager _netManager;

        /// <summary>
        /// White list.
        /// </summary>
        private readonly List<IPAddress> _whiteList;

        /// <summary>
        /// Session dictonary.
        /// key: session id.
        /// value: UDP Session interface.
        /// </summary>
        private readonly ConcurrentDictionary<int, IUdpSession> _sessionDict;

        // Call back Delegates for network listener.
        protected abstract void OnRequestConnection();
        protected abstract void OnConnected(IUdpSession session);
        protected abstract void OnNetError();
        protected abstract void OnDisconnected(IUdpSession session);

		public UdpServer(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpServer>>();

            // Initialize network.
            _packetProcessor = new NetPacketProcessor();
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener)
            {
                BroadcastReceiveEnabled = true,
                ReuseAddress = false,
                UnsyncedEvents = true,
            };

			// Black list.
			_whiteList = new List<IPAddress>();
        }

        /// <summary>
        /// Start upd network.
        /// </summary>
        public void Start()
        {
			// Initialize listener.
			Initialize_Listener();

			// Register UDP packet.
			_packetProcessor.SubscribeReusable<UdpPacket, IUdpSession>(HandlePacket);

			// Start server.
			_netManager.Start();
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
						peer.Disconnect();
					}
				}

				// Add session.
				var session = new UdpSession
				{
					SessionId = peer.Id,
					Address = peer.Address,
					Peer = peer,
				};

				// Check session id duplicate.
				_sessionDict.AddOrUpdate(peer.Id, session, (key, old) => session);

				// OnConnected.
				OnConnected(session);
			};

			// OnReceive.
			_listener.NetworkReceiveEvent += (NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) =>
			{
				if (false == _sessionDict.TryGetValue(peer.Id, out var session))
				{
					_logger.LogError($"Session not found. id: {peer.Id} / peer: {peer}");
					peer.Disconnect();
					return;
				}

				// Read all packets.
				_packetProcessor.ReadAllPackets(reader, session);
			};

			// OnNetError.
			_listener.NetworkErrorEvent += (endPoint, socketError) =>
			{
				OnNetError();
			};

			// OnDisconnected.
			_listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
			{
				OnDisconnected(new UdpSession
				{
					SessionId = peer.Id,
					Address = peer.Address,
				});
			};
		}
        
        /// <summary>
        /// Handle udp packet.
        /// </summary>
		private void HandlePacket(UdpPacket packet, IUdpSession session)
        {

        }
    }
}
