
using MemoryPack;
using System.Runtime.InteropServices;
using System.Numerics;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_Dash from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_Dash : NetPacketReqBase
    {
        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }
    }

    /// <summary>
    /// NetPacket_Dash from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_Dash
    {
        /// <summary>
        /// 대시 취소 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Vector2 Position { get; set; }
    }
}
