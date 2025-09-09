
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    /// <summary>
    /// HubPacketCS_EnterZone.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.CS_EnterZone)]
    public partial class HubPacketCS_EnterZone : HubPacketBase_CS
    {
        /// <summary>
        /// ZoneId.
        /// </summary>
        [MemoryPackOrder(1)]
        public string ZoneId { get; set; }
    }

    /// <summary>
    /// HubPacketSC_EnterZone.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(HubPacketCode.SC_EnterZone)]
    public partial class HubPacketSC_EnterZone : HubPacketBase_SC
    {
        public override void Dispose()
        {
        }
    }
}
