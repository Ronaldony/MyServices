
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    using GameCore.Datas.Post;

    /// <summary>
    /// HubPacketCS_FailRetrievePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_FailRetrievePost)]
    public partial class HubPacketCS_FailRetrievePost : HubPacketBase_CS
    {
    }

    /// <summary>
    /// HubPacketSC_FailRetrievePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_FailRetrievePost)]
    public partial class HubPacketSC_FailRetrievePost : HubPacketBase_SC
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
