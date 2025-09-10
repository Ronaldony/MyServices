
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;

namespace GamePackets
{

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_Enter : GamePacketHandler
    {
        private readonly GamePacketCS_EnterPlayer _sendPacket;

        public UdpHandler_Enter()
            : base(GamePacketCode.SC_Enter)
        {
            _sendPacket = new GamePacketCS_EnterPlayer();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var gamePacketEnter = MemoryPackDataSerializer.Deserialize<GamePacketSC_Enter>(packet.Data);
            var playerDataManager = PlayerDataManager.Instance;

            playerDataManager.PlayInfo = gamePacketEnter.PlayInfo;
            playerDataManager.InventoryInfo = gamePacketEnter.InventoryInfo;

            // Load scene.
            var loginActor = LoginActor.Instance;   
            loginActor.LoadScene(playerDataManager.PlayInfo.Location.Area.ToString());
        }
    }
}
