using GameCore.Datas.Enemy;
using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacketCS_HitEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_HitEnemy : NetPacketBase_CS
    {
    }

    /// <summary>
    /// NetPacketSC_HitEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_HitEnemy : NetPacketBase_SC
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
