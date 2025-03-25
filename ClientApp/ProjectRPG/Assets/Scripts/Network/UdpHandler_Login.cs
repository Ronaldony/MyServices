
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Server_World.Server.World.Handlers
{

    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_Login : UdpHandler
    {
        private readonly NetPacketCS_LoadPlayer _sendPacket;

        public UdpHandler_Login()
            : base(GamePacketCode.SC_Login)
        {
            _sendPacket = new NetPacketCS_LoadPlayer();
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

            playerDataManager.PlayInfo = packetData.PlayInfo;
            playerDataManager.InventoryInfo = packetData.InventoryInfo;

            netPacketManager.Send<NetPacketCS_LoadPlayer>(GamePacketCode.CS_LoadPlayer, _sendPacket);
        }
    }
}
