
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
        private readonly GamePacketCS_SpawnUnits _sendPacket;

        public UdpHandler_Enter()
            : base(GamePacketCode.SC_Enter)
        {
            _sendPacket = new GamePacketCS_SpawnUnits();
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

            // Spawn units.
            GamePacketManager.Instance.Send<GamePacketCS_SpawnUnits>(GamePacketCode.CS_SpawnUnits, _sendPacket);

            // Load scene.
            var loginActor = LoginActor.Instance;
            loginActor.LoadScene(playerDataManager.PlayInfo.Location.Area.ToString());
        }
    }
}
