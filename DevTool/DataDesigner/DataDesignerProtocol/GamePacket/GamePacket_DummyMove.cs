
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketCS_DummyMove.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_DummyMove)]
    public partial class GamePacketCS_DummyMove : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_DummyMove.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_DummyMove)]
    public partial class GamePacketSC_DummyMove : GamePacketBase_SC
    {
        public GamePacketSC_DummyMove()
        {
            PlayInfos = new Dictionary<long, DTO_PlayInfo>();
            Mobs = new Dictionary<long, DTO_Mob>();
        }

        /// <summary>
        /// 유저 플레이 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// 적 정보.
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
