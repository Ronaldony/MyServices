
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    /// <summary>
    /// HubPacketCS_DeletePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_DeletePost)]
    public partial class HubPacketCS_DeletePost : HubPacketBase_CS
    {
        /// <summary>
        /// 삭제 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }
    }

    /// <summary>
    /// HubPacketSC_DeletePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_DeletePost)]
    public partial class HubPacketSC_DeletePost : HubPacketBase_SC
    {
        /// <summary>
        /// 삭제 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(0)]
        public string PostUid { get; set; }

        /// <summary>
        /// 삭제 여부.
        /// </summary>
        [MemoryPackOrder(1)]
        public bool IsDeleted { get; set; }

        public override void Dispose()
        {
        }
    }
}
