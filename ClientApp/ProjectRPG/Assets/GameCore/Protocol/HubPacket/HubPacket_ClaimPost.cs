
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    /// <summary>
    /// HubPacketCS_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_ClaimPost)]
    public partial class HubPacketCS_ClaimPost : HubPacketBase_CS
    {
        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }
    }

    /// <summary>
    /// HubPacketSC_ClaimPost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_ClaimPost)]
    public partial class HubPacketSC_ClaimPost : HubPacketBase_SC
    {
        /// <summary>
        /// 수령 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsClaimed { get; set; }

        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }

        public override void Dispose()
        {
        }
    }
}
