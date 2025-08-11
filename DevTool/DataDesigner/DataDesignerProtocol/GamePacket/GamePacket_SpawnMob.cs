
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
        public GamePacketSC_SpawnMob()
        {
            Mobs = new Dictionary<long, DTO_Mob>();
        }

        /// <summary>
        /// Mobs.
        /// key: unit id.
        /// value: Mob.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, DTO_Mob> Mobs { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            Mobs.Clear();
        }
    }
}
