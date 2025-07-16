using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacketCS_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_Attack : NetPacketBase_CS
    {
        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Angle { get; set; }

        /// <summary>
        /// 무기 데미지.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Damage { get; set; }
    }

    /// <summary>
    /// NetPacketSC_Attack.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_Attack : NetPacketBase_SC
    {
        /// <summary>
        /// 공격 Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Angle { get; set; }
    }
}
