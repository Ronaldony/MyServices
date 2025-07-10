using GameCore.Datas.Play;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.NetPacket
{
    /// <summary>
    /// NetPacketCS_ChangeEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketCS_ChangeEquipment : NetPacketBase_CS
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PlayInfo PlayInfo { get; set; }
    }

    /// <summary>
    /// NetPacketSC_ChangeEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class NetPacketSC_ChangeEquipment : NetPacketBase_SC
    {
    }
}
