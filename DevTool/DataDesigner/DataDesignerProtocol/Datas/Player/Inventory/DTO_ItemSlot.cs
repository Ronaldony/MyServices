using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Player.Inventory
{
    /// <summary>
    /// DTO_ItemSlot.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_ItemSlot
    {
        public DTO_ItemSlot()
        {

        }

        /// <summary>
        /// TB_Item id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int RefId { get; init; }

        /// <summary>
        /// Count.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Count { get; init; }
    }
}
