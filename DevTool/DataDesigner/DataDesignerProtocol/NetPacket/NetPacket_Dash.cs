
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
    }

    /// <summary>
    /// NetPacket_Dash from server to client.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketSC_Dash
    {
        /// <summary>
        /// 대시 플레이어.
        /// </summary>
        [MemoryPackOrder(0)]
        public string Pid { get; set; }

        /// <summary>
        /// 대시 위치.
        /// </summary>
        [MemoryPackOrder(1)]
        public Type_MoveDirection DashDirection { get; set; }
    }
}
