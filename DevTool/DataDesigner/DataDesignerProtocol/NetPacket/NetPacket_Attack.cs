using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_Attack from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_Attack : NetPacketBase
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
    public partial record NetPacketSC_Attack : NetPacketBase
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
        public int Damage { get; set; }

        /// <summary>
        /// Attack id.
        /// </summary>
        [MemoryPackOrder(3)]
        public Type_MoveDirection MoveDirection { get; set; }
    }
}
