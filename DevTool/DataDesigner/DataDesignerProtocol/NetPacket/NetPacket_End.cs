using GameCore.NetPacket;
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.Packet
{
    /// <summary>
    /// NetPacket_MoveEnd from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_MoveEnd : NetPacketBase
    {
        /// <summary>
        /// Last position.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 LastPos { get; init; }

        /// <summary>
        /// PacketTime.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime PacketTime { get; init; }
    }

    /// <summary>
    /// NetPacket_MoveEnd from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_MoveEnd : NetPacketBase
    {
        /// <summary>
        /// 마지막 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 EndPos { get; init; }
    }
}
