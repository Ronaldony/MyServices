using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Play;

    /// <summary>
    /// GamePacketCS_ChangeEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_SiwtchEquipment)]
    public partial class GamePacketCS_ChangeEquipment : GamePacketBase_CS
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_PlayInfo PlayInfo { get; set; }
    }

    /// <summary>
    /// GamePacketSC_ChangeEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_SiwtchEquipment)]
    public partial class GamePacketSC_ChangeEquipment : GamePacketBase_SC
    {
    }
}
