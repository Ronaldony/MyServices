using GameCore.Datas.Plays;
using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_UpdateStat.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_UpdateUnitStat)]
    public partial class GamePacketCS_UpdateUnitStat : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_UpdateStat.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateUnitStat)]
    public partial class GamePacketSC_UpdateUnitStat : GamePacketBase_SC
    {
        /// <summary>
        /// Unit stats.
        /// key: unit id.
        /// value: DTO_Stat. 
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Stat> UnitStats { get; set; }
    }
}
