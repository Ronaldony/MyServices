
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// DomainPacketCS_DeletePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.CS_DeletePost)]
    public partial class DomainPacketCS_DeletePost : DomainPacketBase_CS
    {
        /// <summary>
        /// 삭제 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }
    }

    /// <summary>
    /// DomainPacketSC_DeletePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.SC_DeletePost)]
    public partial class DomainPacketSC_DeletePost : DomainPacketBase_SC
    {
        /// <summary>
        /// 삭제 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsDeleted { get; set; }

        public override void Dispose()
        {
        }
    }
}
