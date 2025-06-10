using MemoryPack;
using System.Runtime.InteropServices;
using System;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_MoveStart from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_MoveStart : NetPacketBase_CS
    {
        /// <summary>
        /// MoveDirection.
        /// </summary>
        [MemoryPackOrder(0)]
        public Type_MoveDirection MoveDirection { get; set; }

        /// <summary>
        /// Packet time.
        /// </summary>
        [MemoryPackOrder(1)]
        public DateTime PacketTime { get; set; }
    }

    /// <summary>
    /// NetPacket_MoveStart from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_MoveStart
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// MoveDirection.
        /// </summary>
        [MemoryPackOrder(1)]
        public Type_MoveDirection MoveDirection { get; set; }
    }
}
