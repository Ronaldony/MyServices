
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Post;

    /// <summary>
    /// DomainPacketCS_RetrievePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.CS_RetrievePost)]
    public partial class DomainPacketCS_RetrievePost : DomainPacketBase_CS
    {
        /// <summary>
        /// 수령 우편물.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_Post Post { get; set; }
    }

    /// <summary>
    /// DomainPacketSC_RetrievePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.SC_RetrievePost)]
    public partial class DomainPacketSC_RetrievePost : DomainPacketBase_SC
    {
        /// <summary>
        /// 수령 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsRetrieved { get; set; }

        public override void Dispose()
        {
        }
    }
}
