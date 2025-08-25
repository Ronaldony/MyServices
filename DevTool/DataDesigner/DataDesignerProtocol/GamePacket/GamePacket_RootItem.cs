
using MemoryPack;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Items;

    /// <summary>
    /// GamePacketCS_RootItem.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_RootItem)]
    public partial class GamePacketCS_RootItem : GamePacketBase_CS
    {
        /// <summary>
        /// Drop id.
        /// </summary>
        [MemoryPackOrder(2)]
        public long DropId { get; set; }

        /// <summary>
        /// Root item.
        /// </summary>
        [MemoryPackOrder(3)]
        public DTO_Item Item { get; set; }
    }

    /// <summary>
    /// GamePacketSC_RootItem.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_RootItem)]
    public partial class GamePacketSC_RootItem : GamePacketBase_SC
    {
        /// <summary>
        /// Root 실패 여부.
        /// </summary>
        [MemoryPackOrder(0)]
        public bool IsFailed { get; set; }

        /// <summary>
        /// Root item.
        /// </summary>
        [MemoryPackOrder(1)]
        public DTO_Item Item { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
