
using MemoryPack;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GameCore.GamePacket
{
    using GameCore.Datas.Items;

    /// <summary>
    /// GamePacketCS_DropItems.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.CS_DropItems)]
    public partial class GamePacketCS_DropItems : GamePacketBase_CS
    {
    }

    /// <summary>
    /// GamePacketSC_DropItems.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    [GamePacketAttribute(GamePacketCode.SC_DropItems)]
    public partial class GamePacketSC_DropItems : GamePacketBase_SC
    {
        public GamePacketSC_DropItems()
        {
            DropItems = new Dictionary<long, DTO_Item>();
        }

        /// <summary>
        /// UnitId.
        /// </summary>
        [MemoryPackOrder(0)]
        public long UnitId { get; set; }

        /// <summary>
        /// Drop items.
        /// </summary>
        [MemoryPackOrder(1)]
        public Dictionary<long, DTO_Item> DropItems { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            DropItems.Clear();
        }
    }
}
