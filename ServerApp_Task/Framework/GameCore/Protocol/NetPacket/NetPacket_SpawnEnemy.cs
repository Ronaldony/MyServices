
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Enemy;

    /// <summary>
    /// NetPacketSC_SpawnEnemy.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_SpawnEnemy : NetPacketBase_SC
    {
        /// <summary>
        /// 적 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
