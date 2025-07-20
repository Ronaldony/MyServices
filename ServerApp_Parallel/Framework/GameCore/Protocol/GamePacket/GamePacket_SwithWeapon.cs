using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_SwitchWeapon.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_SwitchWeapon : GamePacketBase_CS
    {
        /// <summary>
        /// Switch weapon id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int WeaponId { get; set; }
    }

    /// <summary>
    /// GamePacketSC_SwitchWeapon.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_SwitchWeapon : GamePacketBase_SC
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Switch weapon id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int WeaponId { get; set; }
    }
}
