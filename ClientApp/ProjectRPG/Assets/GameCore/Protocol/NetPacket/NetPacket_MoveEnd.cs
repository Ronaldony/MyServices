using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;
using System;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_MoveEnd from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_MoveEnd : NetPacketReqBase
    {
        /// <summary>
        /// Last position.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 EndPos { get; set; }

        /// <summary>
        /// PacketTime.
        /// </summary>
        [MemoryPackOrder(2)]
        public DateTime PacketTime { get; set; }
    }

    /// <summary>
    /// NetPacket_MoveEnd from client to server.
    /// </summary>

    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_MoveEnd
    {
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 마지막 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 EndPos { get; set; }
    }
}
