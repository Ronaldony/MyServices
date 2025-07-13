using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Enemy;

    /// <summary>
    /// NetPacketSC_UpdateMovePattern.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_UpdateMovePattern : NetPacketBase_SC
    {
        /// <summary>
        /// Enemies updated move pattern.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
