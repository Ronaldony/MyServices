using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Enemy;

    /// <summary>
    /// GamePacketSC_UpdateMovePattern.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_UpdateMovePattern : GamePacketBase_SC
    {
        /// <summary>
        /// Enemies updated move pattern.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
