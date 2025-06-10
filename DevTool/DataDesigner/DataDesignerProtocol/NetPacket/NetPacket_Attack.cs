using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_Attack from client to server.
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
    /// NetPacket_Attack from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_Attack
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
