
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
    public class UdpHandler_Login : UdpHandler
    {
        private readonly GamePacketCS_SpawnPlayer _sendPacket;

        public UdpHandler_Login()
            : base(GamePacketCode.SC_Login)
        {
            _sendPacket = new GamePacketCS_SpawnPlayer();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - Login");

            var packetData = MemoryPackDataSerializer.Deserialize<GamePacketSC_Login>(packet.Data);

            var playerDataManager = PlayerDataManager.Instance;
            var gamePacketManager = GamePacketManager.Instance;
            var loginActor = LoginActor.Instance;

            playerDataManager.PlayInfo = packetData.PlayInfo;
            playerDataManager.InventoryInfo = packetData.InventoryInfo;

            // Load scene.
            loginActor.LoadScene(playerDataManager.PlayInfo.Location.Area.ToString());
            //switch (playerDataManager.PlayInfo.Area)
            //{
            //    case Type_Area.HuntZone:
            //        loginActor.LoadScene(Type_Area.HuntZone.ToString());
            //        break;
            //    case Type_Area.BattleZone:
            //        loginActor.LoadScene(Type_Area.BattleZone.ToString());
            //        break;
            //    case Type_Area.Village:
            //        loginActor.LoadScene(Type_Area.Village.ToString());
            //        break;
            //}

            // Delay.
            gamePacketManager.Send<GamePacketCS_SpawnPlayer>(GamePacketCode.CS_SpawnPlayer, _sendPacket);
        }
    }
}
