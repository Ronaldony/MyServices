namespace ServerEngine.Test.Network
{
    using GameCore;
    using Server_Game.Core.Utilities;
    using Server_Game.Server.Game;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// UdpHandler_Test.
    /// </summary>
    public class UdpHandler_Move : UdpHandler
    {
        private readonly GameServer _gameServer;

        public UdpHandler_Move()
            : base(GamePacketCode.Move)
        {
            _gameServer = ServiceUtility.GetService<GameServer>();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(int sessionId, ulong sessionUid, UdpPacket packet)
		{

        }
    }
}
