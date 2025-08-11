
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketSC_UpdateSection.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateSection)]
    public partial class GamePacketSC_UpdateSection : GamePacketBase_SC
    {
        public GamePacketSC_UpdateSection()
        {
            PlayInfos = new ConcurrentDictionary<long, DTO_PlayInfo>();
            Mobs = new ConcurrentDictionary<long, DTO_Mob>();
        }

        /// <summary>
        /// 유저 플레이 정보.
        /// key: unit id.
        /// value: PlayInfo.
        /// </summary>
        [MemoryPackOrder(0)]
        public ConcurrentDictionary<long, DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// 몹 정보.
        /// key: unit id.
        /// value: Mob.
        /// </summary>
        [MemoryPackOrder(1)]
        public ConcurrentDictionary<long, DTO_Mob> Mobs { get; set; }

        public new void Dispose()
        {
            PlayInfos.Clear();
            Mobs.Clear();
        }
    }
}
