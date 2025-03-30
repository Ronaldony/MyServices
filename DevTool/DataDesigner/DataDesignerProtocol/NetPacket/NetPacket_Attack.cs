using MemoryPack;
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
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int AttackId { get; set; }

        /// <summary>
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(2)]
        public Type_MoveDirection MoveDirection { get; set; }
    }

    /// <summary>
    /// NetPacket_Attack from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_Attack
    {
        /// <summary>
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int AttackId { get; set; }

        /// <summary>
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Damage { get; set; }

        /// <summary>
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(2)]
        public Type_MoveDirection MoveDirection { get; set; }
    }
}
