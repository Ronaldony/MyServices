
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// DomainPacketCS_DeletePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.CS_DeletePostAll)]
    public partial class DomainPacketCS_DeletePostAll : DomainPacketBase_CS
    {
    }

    /// <summary>
    /// DomainPacketSC_DeletePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.SC_DeletePostAll)]
    public partial class DomainPacketSC_DeletePostAll : DomainPacketBase_SC
    {
        public override void Dispose()
        {
        }
    }
}
