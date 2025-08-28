
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    using GameCore.Datas.Post;
    using GameCore.HubPacket;

    /// <summary>
    /// HubPacketCS_RetrievePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_RetrievePost)]
    public partial class HubPacketCS_RetrievePost : HubPacketBase_CS
    {
        /// <summary>
        /// 수령 우편물 Uid.
        /// </summary>
        [MemoryPackOrder(1)]
        public string PostUid { get; set; }
    }

    /// <summary>
    /// HubPacketSC_RetrievePost.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_RetrievePost)]
    public partial class HubPacketSC_RetrievePost : HubPacketBase_SC
    {
        /// <summary>
        /// 수령 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsRetrieved { get; set; }

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
