
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    using GameCore.Datas.Player;
    using GameCore.HubPacket;

    /// <summary>
    /// HubPacketCS_Enter.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_Enter)]
    public partial class HubPacketCS_Enter : HubPacketBase_CS
    {
        /// <summary>
        /// Token.
        /// </summary>
        [MemoryPackOrder(1)]
        public string Token { get; set; }
    }

    /// <summary>
    /// HubPacketSC_Enter.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_Enter)]
    public partial class HubPacketSC_Enter : HubPacketBase_SC
    {
        /// <summary>
        /// 우편함 정보.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PostInfo PostInfo { get; set; }

        public override void Dispose()
        {
        }
    }
}
