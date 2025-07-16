
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// NetPacketCS_SpawnPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_SpawnPlayer : NetPacketBase_CS
    {
    }

    /// <summary>
    /// NetPacketSC_SpawnPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_SpawnPlayer : NetPacketBase_SC, IDisposable
    {
        public NetPacketSC_SpawnPlayer()
        {
            PlayInfos = new List<DTO_PlayInfo>();
            Enemies = new List<DTO_Enemy>();
        }

        /// <summary>
        /// 유저 플레이 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// 적 정보.
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
