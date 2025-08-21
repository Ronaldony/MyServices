using GameCore.Datas.Plays;
using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    /// <summary>
    /// GamePacketCS_SwitchEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_SwitchEquipment)]
    public partial class GamePacketCS_SwitchEquipment : GamePacketBase_CS
    {
        /// <summary>
        /// Equipment.
        /// </summary>
        [MemoryPackOrder(2)]
        public DTO_Equipment Equipment { get; set; }
    }

    /// <summary>
    /// GamePacketSC_SwitchEquipment.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_SwitchEquipment)]
    public partial class GamePacketSC_SwitchEquipment : GamePacketBase_SC
    {
        /// <summary>
        /// Unit id.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// Equipment.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_Equipment Equipment { get; set; }

        public override void Dispose()
        {
        }
    }
}
