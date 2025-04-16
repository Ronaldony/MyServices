
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

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
}
