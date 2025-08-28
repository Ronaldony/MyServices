
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    using GameCore.Datas.Post;
    using GameCore.HubPacket;

    /// <summary>
    /// HubPacketCS_PushPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_PushPost)]
    public partial class HubPacketCS_PushPost : HubPacketBase_CS
    {
    }

    /// <summary>
    /// HubPacketSC_PushPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_PushPost)]
    public partial class HubPacketSC_PushPost : HubPacketBase_SC
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
