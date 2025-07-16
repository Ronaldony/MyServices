
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketCS_SpawnPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_SpawnPlayer : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_SpawnPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_SpawnPlayer : GamePacketBase_SC, IDisposable
    {
        public GamePacketSC_SpawnPlayer()
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
