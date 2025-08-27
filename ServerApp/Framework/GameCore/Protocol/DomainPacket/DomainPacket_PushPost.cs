
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Post;

    /// <summary>
    /// DomainPacketCS_PushPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.CS_PushPost)]
    public partial class DomainPacketCS_PushPost : DomainPacketBase_CS
    {
    }

    /// <summary>
    /// DomainPacketSC_PushPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(DomainPacketCode.SC_PushPost)]
    public partial class DomainPacketSC_PushPost : DomainPacketBase_SC
    {
        /// <summary>
        /// 푸시된 우편물.
        /// </summary>
        [MemoryPackOrder(0)]
        public List<DTO_Post> PushPosts { get; set; }

        public override void Dispose()
        {
        }
    }
}
