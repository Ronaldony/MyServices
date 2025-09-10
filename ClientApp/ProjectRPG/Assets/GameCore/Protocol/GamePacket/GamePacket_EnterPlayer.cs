
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketCS_EnterPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_EnterPlayer)]
    public partial class GamePacketCS_EnterPlayer : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_EnterPlayer.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_EnterPlayer)]
    public partial class GamePacketSC_EnterPlayer : GamePacketBase_SC
    {
        public GamePacketSC_EnterPlayer()
        {
            PlayInfos = new Dictionary<long, DTO_PlayInfo>();
            Mobs = new Dictionary<long, DTO_Mob>();
        }

        /// <summary>
        /// UnitId.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// Zone id.
        /// </summary>
        [MemoryPackOrder(1)]
        public string ZoneId { get; set; }

        /// <summary>
        /// 유저 플레이 정보.
        /// key: unit id.
        /// value: PlayInfo.
        /// </summary>
        [MemoryPackOrder(2)]
        public Dictionary<long, DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// 몹 정보.
        /// key: unit id.
        /// value: Mob.
        /// </summary>
        [MemoryPackOrder(3)]
        public Dictionary<long, DTO_Mob> Mobs { get; set; }

        public override void Dispose()
        {
            PlayInfos.Clear();
            Mobs.Clear();
        }
    }
}
