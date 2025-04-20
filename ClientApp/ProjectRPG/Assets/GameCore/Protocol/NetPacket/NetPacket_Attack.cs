using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_Attack from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_Attack : NetPacketReqBase
    {
        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 AttackAngle { get; set; }
    }

    /// <summary>
    /// NetPacket_Attack from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_Attack
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
        public Vector2 AttackAngle { get; set; }
    }
}
