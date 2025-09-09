
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using GameCore.HubPacket;
using GamePackets;
using ServerEngine.Network.UDP;

namespace HubPackets
{
    /// <summary>
    /// UdpHandler_DeletePost.
    /// </summary>
    public class UdpHandler_Enter : HubPacketHandler
    {
        private readonly HubPacketCS_Enter _sendPacket;

        public UdpHandler_Enter()
            : base(HubPacketCode.SC_Enter)
        {
            _sendPacket = new HubPacketCS_Enter();
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            var playerDataManger = PlayerDataManager.Instance;

            var packetEnter = MemoryPackDataSerializer.Deserialize<HubPacketSC_Enter>(packet.Data);
            playerDataManger.PostInfo = packetEnter.PostInfo;

            // Load posts.
            PostBox.Instance.AddUpdatePacket(new UpdateHubPacket
            {
                PacketCode = HubPacketCode.SC_Enter,
                Data = packetEnter,
            });

            // Enter GameServer.
            GamePacketManager.Instance.Send<GamePacketCS_Enter>(GamePacketCode.CS_Enter, new GamePacketCS_Enter
            {
                Pid = playerDataManger.Pid,
                Token = playerDataManger.AccessToken,
            });
        }
    }
}
