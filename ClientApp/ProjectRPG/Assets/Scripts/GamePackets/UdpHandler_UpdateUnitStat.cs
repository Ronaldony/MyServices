
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
            var enemyManagement = EnemyManagement.Instance;
            var dummyManagement = DummyManagement.Instance;
            var playerManagement = PlayerManagement.Instance;

            var gamePacket = MemoryPackDataSerializer.Deserialize<GamePacketSC_UpdateUnitStat>(packet.Data);
            var updatePacket = new UpdateGamePacket
            {
                PacketCode = GamePacketCode.SC_UpdateUnitStat,
                Data = gamePacket,
            };

            Debug.Log($"HandlePacket - SC_UpdateUnitStat");

            playerManagement.AddUpdatePacket(updatePacket);
            enemyManagement.AddUpdatePacket(updatePacket);
            dummyManagement.AddUpdatePacket(updatePacket);
        }
    }
}
