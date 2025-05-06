using GameCore.Datas.Enemy;
using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_HitEnemy from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_HitEnemy : NetPacketReqBase
    {
    }

    /// <summary>
    /// NetPacket_HitEnemy from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_HitEnemy
    {
        /// <summary>
        /// Damage.
        /// </summary>
        [MemoryPackOrder(0)]
        public int Damage { get; set; }

        /// <summary>
        /// Position.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }

        /// <summary>
        /// Enemies.
        /// </summary>
        [MemoryPackOrder(2)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
