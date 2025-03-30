
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Enemy;

    /// <summary>
    /// NetPacket_SpawnEnemy from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_SpawnEnemy
    {
        /// <summary>
        /// 적 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
