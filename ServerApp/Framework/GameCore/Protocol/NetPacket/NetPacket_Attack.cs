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
        /// Damage.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Damage { get; set; }

        /// <summary>
        /// Attack angle.
        /// </summary>
        [MemoryPackOrder(2)]
        public Vector2 AttackAngle { get; set; }
    }

    /// <summary>
    /// NetPacket_Attack from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_Attack
    {
        /// <summary>
        /// 공격 취소 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsCanceled { get; set; }
    }
}
