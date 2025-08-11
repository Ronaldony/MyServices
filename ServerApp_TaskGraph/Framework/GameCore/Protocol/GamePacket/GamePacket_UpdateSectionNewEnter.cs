
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketSC_UpdateSectionNewEnter.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateSectionNewEnter)]
    public partial class GamePacketSC_UpdateSectionNewEnter : GamePacketBase_SC
    {
        public GamePacketSC_UpdateSectionNewEnter()
        {
            PlayInfos = new List<DTO_PlayInfo>();
            Mobs = new List<DTO_Mob>();
        }

        /// <summary>
        /// 유저 플레이 정보.
        /// key: unit id.
        /// value: PlayInfo.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// 몹 정보.
        /// key: unit id.
        /// value: Mob.
        /// </summary>
        [MemoryPackOrder(1)]
        public List<DTO_Mob> Mobs { get; set; }

        public new void Dispose()
        {
            PlayInfos.Clear();
            Mobs.Clear();
        }
    }
}
