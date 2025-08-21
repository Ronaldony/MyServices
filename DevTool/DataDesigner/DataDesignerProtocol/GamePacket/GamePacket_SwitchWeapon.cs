using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_SwitchWeapon.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_SwitchWeapon)]
    public partial class GamePacketCS_SwitchWeapon : GamePacketBase_CS
    {
        /// <summary>
        /// Weapon id.
        /// </summary>
        [MemoryPackOrder(2)]
        public int WeaponId { get; set; }
    }

    /// <summary>
    /// GamePacketSC_SwitchWeapon.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_SwitchWeapon)]
    public partial class GamePacketSC_SwitchWeapon : GamePacketBase_SC
    {
        /// <summary>
        /// Unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// Switch weapon id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int WeaponId { get; set; }

        public override void Dispose()
        {
        }
    }
}
