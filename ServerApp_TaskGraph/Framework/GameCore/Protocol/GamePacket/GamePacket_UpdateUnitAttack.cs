using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Numerics;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketSC_UpdateUnitAttack.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_UpdateUnitAttack)]
    public partial class GamePacketSC_UpdateUnitAttack : GamePacketBase_SC
    {
        public GamePacketSC_UpdateUnitAttack()
        {
            AttackAngles = new Dictionary<long, Vector2>();
        }

        /// <summary>
        /// Mobs to update roam.
        /// key: unit id.
        /// value: attack angle.
        /// </summary>
        [MemoryPackOrder(0)]
        public Dictionary<long, Vector2> AttackAngles { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public new void Dispose()
        {
            AttackAngles.Clear();
        }
    }
}
