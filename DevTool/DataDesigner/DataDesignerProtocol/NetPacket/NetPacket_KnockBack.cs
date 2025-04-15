
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{

    /// <summary>
    /// NetPacket_KnockBack from client to server.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial record NetPacketCS_KnockBack : NetPacketReqBase
    {
    }
}
