
using Externals.Serializer;
using Externals.UDP;
using GameCore;
using GameCore.NetPacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// UdpHandler_Move.
    /// </summary>
    public class UdpHandler_SpawnPlayer : UdpHandler
    {
        public UdpHandler_SpawnPlayer()
            : base(NetPacketCode.SC_SpawnPlayer)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            Debug.Log("HandlePacket - SpawnPlayer");

            var packetData = MemoryPackDataSerializer.Deserialize<NetPacketSC_SpawnPlayer>(packet.Data);

            var areaDataManager = AreaDataManager.Instance;
            var playerDataManager = PlayerDataManager.Instance;
            var loginActor = LoginActor.Instance;

            areaDataManager.SetPlayInfos(packetData.Users);

            // Load scene.
            switch (playerDataManager.PlayInfo.Position.Area)
            {
                case Type_Area.HuntZone:
                    loginActor.LoadScene(Type_Area.HuntZone.ToString());
                    break;
                case Type_Area.BattleZone:
                    loginActor.LoadScene(Type_Area.BattleZone.ToString());
                    break;
                case Type_Area.Village:
                    loginActor.LoadScene(Type_Area.Village.ToString());
                    break;
            }
        }
    }
}
