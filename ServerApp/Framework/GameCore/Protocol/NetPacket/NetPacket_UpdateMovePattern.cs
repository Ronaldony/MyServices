using GameCore.Datas.Enemy;
using MemoryPack;
using System;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacketSC_UpdateMovePattern.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_UpdateMovePattern
    {
        /// <summary>
        /// Enemies updated move pattern.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
