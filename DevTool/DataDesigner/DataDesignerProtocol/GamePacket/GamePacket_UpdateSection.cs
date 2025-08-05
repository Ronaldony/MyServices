
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
