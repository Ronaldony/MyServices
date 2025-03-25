namespace Server_World.Server.World.Handlers
{
    using GameCore;
    using Server_World.Core.Utilities;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_MoveStart : UdpHandler
    {
        private readonly WorldServer _gameServer;

        public UdpHandler_MoveStart(IServiceProvider serviceProvider)
            : base(GamePacketCode.CS_MoveStart)
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
