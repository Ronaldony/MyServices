using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacketCS_SwitchWeapon.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_SwitchWeapon : NetPacketBase_CS
    {
        /// <summary>
        /// Switch weapon id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int WeaponId { get; set; }
    }

    /// <summary>
    /// NetPacketSC_SwitchWeapon.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_SwitchWeapon : NetPacketBase_SC
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
