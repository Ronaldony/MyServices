using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using GameCore.HubPacket;
using HubPackets;
using ServerEngine.Network.UDP;

namespace GamePackets
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
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_EnterPlayer,
                Data = packetData,
            };

            var playerDataManager = PlayerDataManager.Instance;

            playerDataManager.UnitId = packetData.UnitId;
            PlayerManagement.Instance.UnitId = packetData.UnitId;

            EnemyManagement.Instance.AddUpdatePacket(updatePacket);
            DummyManagement.Instance.AddUpdatePacket(updatePacket);

            // Enter zone.
            HubPacketManager.Instance.Send<HubPacketCS_EnterZone>(HubPacketCode.CS_EnterZone, new HubPacketCS_EnterZone
            {
                Pid = playerDataManager.Pid,
                ZoneId = packetData.ZoneId
            });
        }
    }
}
