using MemoryPack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// NetPacketSC_UpdateSection.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_UpdateSection : NetPacketBase_SC, IDisposable
    {
        public NetPacketSC_UpdateSection()
        {
            PlayInfos = new List<DTO_PlayInfo>();
            Enemies = new List<DTO_Enemy>();
        }

        /// <summary>
        /// Play info list.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// Enemies.
        /// </summary>
        [MemoryPackOrder(1)]
        public List<DTO_Enemy> Enemies { get; set; }

        public void Dispose()
        {
            PlayInfos.Clear(); 
            Enemies.Clear();
        }
    }
}
