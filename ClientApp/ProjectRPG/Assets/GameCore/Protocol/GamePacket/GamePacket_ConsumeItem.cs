
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.HubPacket
{
    using GameCore.GamePacket;

    /// <summary>
    /// GamePacketCS_ConsumeItem.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(GamePacketCode.CS_ConsumeItem)]
    public partial class GamePacketCS_ConsumeItem : GamePacketBase_CS
    {
        /// <summary>
        /// TB_Item Id.
        /// </summary>
        [MemoryPackOrder(2)]
        public int RefId { get; set; }
    }

    /// <summary>
    /// GamePacketSC_ConsumeItem.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [HubPacketAttribute(GamePacketCode.SC_ConsumeItem)]
    public partial class GamePacketSC_ConsumeItem : GamePacketBase_SC
    {
        /// <summary>
        /// TB_Item Id.
        /// </summary>
        [MemoryPackOrder(2)]
        public int RefId { get; set; }

        public override void Dispose()
        {
        }
    }
}
