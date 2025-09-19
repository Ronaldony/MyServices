
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    /// <summary>
    /// HubPacketCS_FailClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_ClaimPostFail)]
    public partial class HubPacketCS_ClaimPostFail : HubPacketBase_CS
    {
    }

    /// <summary>
    /// HubPacketSC_FailClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_ClaimPostFail)]
    public partial class HubPacketSC_ClaimPostFail : HubPacketBase_SC
    {
        /// <summary>
        /// 우편물 수령 실패.
        /// </summary>
        [MemoryPackOrder(0)]
        public string PostUid { get; set; }

        public override void Dispose()
        {
        }
    }
}
