
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_Enter : UdpHandler
    {
        private readonly GamePacketCS_SpawnPlayer _sendPacket;

        public UdpHandler_Enter()
            : base(GamePacketCode.SC_Enter)
        {
            _sendPacket = new GamePacketCS_SpawnPlayer();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_Enter>(packet.Data);
            var playerDataManager = PlayerDataManager.Instance;

            playerDataManager.PlayInfo = packetData.PlayInfo;
            playerDataManager.InventoryInfo = packetData.InventoryInfo;

            // Delay.
            var gamePacketManager = GamePacketManager.Instance;

            gamePacketManager.Send<GamePacketCS_SpawnPlayer>(GamePacketCode.CS_SpawnPlayer, _sendPacket);

            //Load scene.
            var loginActor = LoginActor.Instance;
            loginActor.LoadScene(playerDataManager.PlayInfo.Location.Area.ToString());
        }
    }
}
