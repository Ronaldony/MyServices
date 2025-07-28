
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;

    /// <summary>
    /// GamePacketSC_SpawnMob.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_SpawnMob)]
    public partial class GamePacketSC_SpawnMob : GamePacketBase_SC
    {
        /// <summary>
        /// 적 정보.
        /// key: unit id.
        /// value: Mob.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Mob> Mobs { get; set; }
    }
}
