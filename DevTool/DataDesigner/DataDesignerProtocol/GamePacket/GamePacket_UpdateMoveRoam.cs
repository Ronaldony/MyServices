using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Mobs;

    /// <summary>
    /// GamePacketSC_UpdateMovePattern.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateMobRoam)]
    public partial class GamePacketSC_UpdateMobRoam : GamePacketBase_SC
    {
        /// <summary>
        /// Mobs to update roam.
        /// key: unit id.
        /// value: mob.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Mob> Mobs { get; set; }
    }
}
