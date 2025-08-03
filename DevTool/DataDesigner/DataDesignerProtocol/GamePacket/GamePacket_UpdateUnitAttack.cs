using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Numerics;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketSC_UpdateMobAttack.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateUnitAttack)]
    public partial class GamePacketSC_UpdateMobAttack : GamePacketBase_SC
    {
        /// <summary>
        /// Mobs to update roam.
        /// key: unit id.
        /// value: attack angle.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, Vector2> AttackAngles { get; set; }
    }
}
