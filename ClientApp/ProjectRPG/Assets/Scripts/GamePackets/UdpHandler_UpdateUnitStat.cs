
using Externals.Serializer;
using GameCore;
using GameCore.GamePacket;
using ServerEngine.Network.UDP;
using UnityEngine;

namespace GamePackets
{
    /// <summary>
    /// UdpHandler_UpdateUnitStat.
    /// </summary>
    public class UdpHandler_UpdateUnitStat : GamePacketHandler
    {
        public UdpHandler_UpdateUnitStat()
            : base(GamePacketCode.SC_UpdateUnitStat)
        {
        }

        /// <summary>
        /// Handle packet.
        /// </summary>
        public override void HandlePacket(UdpPacket packet)
        {
            // management.
            var gamePacket = MemoryPackDataSerializer.Deserialize<GamePacketSC_UpdateUnitStat>(packet.Data);
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_UpdateUnitStat,
                Data = gamePacket,
            };

            PlayerManagement.Instance.AddUpdatePacket(updatePacket);
            EnemyManagement.Instance.AddUpdatePacket(updatePacket);
            OpponentManagement.Instance.AddUpdatePacket(updatePacket);
        }
    }
}
