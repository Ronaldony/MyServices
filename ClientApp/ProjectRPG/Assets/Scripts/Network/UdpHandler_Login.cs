
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_Login : UdpHandler
    {
        private readonly NetPacketCS_SpawnPlayer _sendPacket;

        public UdpHandler_Login()
            : base(NetPacketCode.SC_Login)
        {
            _sendPacket = new NetPacketCS_SpawnPlayer();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - Login");

            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_Login>(packet.Data);

            var playerDataManager = PlayerDataManager.Instance;
            var netPacketManager = NetPacketManager.Instance;
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
            netPacketManager.Send<NetPacketCS_SpawnPlayer>(NetPacketCode.CS_SpawnPlayer, _sendPacket);
        }
    }
}
