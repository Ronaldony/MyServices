
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketCS_SpawnPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_SpawnPlayer)]
    public partial class GamePacketCS_SpawnPlayer : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_SpawnPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_SpawnPlayer)]
    public partial class GamePacketSC_SpawnPlayer : GamePacketBase_SC
    {
        public GamePacketSC_SpawnPlayer()
        {
            PlayInfos = new Dictionary<long, DTO_PlayInfo>();
            Mobs = new Dictionary<long, DTO_Mob>();
        }

        /// <summary>
        /// 유저 플레이 정보.
        /// key: unit id.
        /// value: PlayInfo.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// 몹 정보.
        /// key: unit id.
        /// value: Mob.
        /// </summary>
        [MemoryPackOrder(1)]
        public Dictionary<long, DTO_Mob> Mobs { get; set; }

        public new void Dispose()
        {
            PlayInfos.Clear();
            Mobs.Clear();
        }
    }
}
