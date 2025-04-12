
using MemoryPack;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Numerics;

namespace GameCore.NetPacket
{

    /// <summary>
    /// NetPacket_KnockBack from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_KnockBack : NetPacketReqBase
    {
        /// <summary>
        /// 넉백 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }
    }

    /// <summary>
    /// NetPacket_KnockBack from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_KnockBack
    {
        /// <summary>
        /// 넉백 취소 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsCanceled { get; set; }
    }
}
