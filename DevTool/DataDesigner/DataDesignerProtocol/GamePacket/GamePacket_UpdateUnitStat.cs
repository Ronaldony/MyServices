using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Plays;

    /// <summary>
    /// GamePacketCS_UpdateUnitStat.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_UpdateUnitStat)]
    public partial class GamePacketCS_UpdateUnitStat : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_UpdateUnitStat.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateUnitStat)]
    public partial class GamePacketSC_UpdateUnitStat : GamePacketBase_SC
    {
        public GamePacketSC_UpdateUnitStat()
        {
            UnitStats = new Dictionary<long, DTO_Stat>();
        }

        /// <summary>
        /// Unit stats.
        /// key: unit id.
        /// value: DTO_Stat. 
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Stat> UnitStats { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public new void Dispose()
        {
            UnitStats.Clear();
        }
    }
}
