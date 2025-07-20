using GameCore.Datas.Enemy;
using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_HitEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_HitEnemy : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_HitEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_HitEnemy : GamePacketBase_SC
    {
        /// <summary>
        /// Damage.
        /// </summary>
        [MemoryPackOrder(0)]
        public int Damage { get; set; }

        /// <summary>
        /// Enemies.
        /// </summary>
        [MemoryPackOrder(1)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
