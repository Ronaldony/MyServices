using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_SwitchWeapon from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_SwitchWeapon : NetPacketReqBase
    {
        /// <summary>
        /// Switch weapon id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int WeaponId { get; set; }
    }

    /// <summary>
    /// NetPacket_SwitchWeapon from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_SwitchWeapon
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
