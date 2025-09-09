
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    using GameCore.Datas.Post;

    /// <summary>
    /// HubPacketCS_FailClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_FailClaimPost)]
    public partial class HubPacketCS_FailClaimPost : HubPacketBase_CS
    {
    }

    /// <summary>
    /// HubPacketSC_FailClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_FailClaimPost)]
    public partial class HubPacketSC_FailClaimPost : HubPacketBase_SC
    {
        /// <summary>
        /// 수령 실패 우편물.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_Post Post { get; set; }

        public override void Dispose()
        {
        }
    }
}
