namespace Server_World.Server.World.Handlers
{
    using GameCore;
    using Server_World.Core.Utilities;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Test.
    /// </summary>
    public class UdpHandler_Move : UdpHandler
    {
        private readonly WorldServer _gameServer;

        public UdpHandler_Move()
            : base(GamePacketCode.Move)
        {
            _gameServer = ServiceUtility.GetService<WorldServer>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{

        }
    }
}
