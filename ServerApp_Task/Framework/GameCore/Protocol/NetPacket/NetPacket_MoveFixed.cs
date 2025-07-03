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
    public partial class NetPacketSC_MoveFixed
	{
        /// <summary>
        /// Pid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 수정 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 FixedPos { get; set; }
    }
}
