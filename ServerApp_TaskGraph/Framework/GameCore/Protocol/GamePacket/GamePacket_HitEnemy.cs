using GameCore.Datas.Plays;
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
    public partial class GamePacketCS_HitUnits : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_HitEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_HitUnits : GamePacketBase_SC
    {
        /// <summary>s
        /// Units.
        /// key: unit id.
        /// value: stat. 
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Stat> Enemies { get; set; }
    }
}
