
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacket_KnockBack from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_KnockBack : NetPacketBase_CS
    {
        /// <summary>
        /// 넉백 위치.
        /// </summary>
        [MemoryPackOrder(0)]
        public Vector2 Position { get; set; }
    }
}
