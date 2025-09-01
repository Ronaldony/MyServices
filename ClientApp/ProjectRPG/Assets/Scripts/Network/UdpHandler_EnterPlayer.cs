using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace Network
{
    /// <summary>
    /// UdpHandler_EnterPlayer.
    /// </summary>
    public class UdpHandler_EnterPlayer : GamePacketHandler
    {
        public UdpHandler_EnterPlayer()
            : base(GamePacketCode.SC_EnterPlayer)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // packet.
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_EnterPlayer>(packet.Data);
            var updatePacket = new UpdatePacket
            {
                PacketCode = GamePacketCode.SC_EnterPlayer,
                Data = packetData,
            };

            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;
            var playerDataManagement = PlayerDataManager.Instance;
            var playerManagemenr = PlayerManagement.Instance;

            playerDataManagement.UnitId = packetData.UnitId;
            playerManagemenr.UnitId = packetData.UnitId;

            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
