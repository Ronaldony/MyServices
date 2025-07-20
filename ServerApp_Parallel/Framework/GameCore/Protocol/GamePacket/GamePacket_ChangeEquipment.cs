using GameCore.Datas.Play;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_ChangeEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketCS_ChangeEquipment : GamePacketBase_CS
    {
        /// <summary>
        /// PlayInfo.
        /// </summary>
        [MemoryPackOrder(0)]
        public DTO_PlayInfo PlayInfo { get; set; }
    }

    /// <summary>
    /// GamePacketSC_ChangeEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class GamePacketSC_ChangeEquipment : GamePacketBase_SC
    {
    }
}
