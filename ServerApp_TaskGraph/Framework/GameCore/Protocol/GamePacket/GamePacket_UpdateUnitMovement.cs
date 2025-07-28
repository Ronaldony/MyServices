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
    [GamePacketAttribute(GamePacketCode.CS_UpdateUnitMovement)]
    public partial class GamePacketCS_UpdateUnitMovement : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_UpdateStat.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateUnitMovement)]
    public partial class GamePacketSC_UpdateUnitMovement : GamePacketBase_SC
    {
        /// <summary>
        /// Unit stats.
        /// key: unit id.
        /// value: stat. 
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Location> UnitLocationDict { get; set; }
    }
}
