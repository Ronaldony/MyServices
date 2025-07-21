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
    public partial class GamePacketCS_HitEnemies : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_HitEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_HitEnemies : GamePacketBase_SC
    {
        /// <summary>
        /// Enemies dictionary.
        /// key: enemy.
        /// value: damages.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<DTO_Enemy, List<int>> Enemies { get; set; }
    }
}
