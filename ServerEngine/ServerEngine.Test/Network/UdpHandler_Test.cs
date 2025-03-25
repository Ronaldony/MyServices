namespace ServerEngine.Test.Network
{
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Network.UDP;
    using ServerEngine.Test.Server;

    /// <summary>
    /// UdpHandler_Test.
    /// </summary>
    public class UdpHandler_Test : UdpHandler
    {
        private readonly ILogger<UdpHandler_Test> _logger;
		private readonly IDataSerializer _dataSerializer;
        private readonly TestServer _testServer;

        public UdpHandler_Test(IServiceProvider serviceProvider)
            : base(UdpPacketCode.Test)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UdpHandler_Test>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _testServer = serviceProvider.GetRequiredService<TestServer>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{
			// Send to session.
			_testServer.Send(sessionId, sessionUid, packet.PacketCode, packet.Data);
        }
    }
}
