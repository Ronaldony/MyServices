using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Datas.Items
{
    /// <summary>
    /// DTO_Item.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class DTO_Item
    {
        /// <summary>
        /// TB_Item id.
        /// </summary>
        [MemoryPackOrder(0)]
        public int RefId { get; set; }

        /// <summary>
        /// Count.
        /// </summary>
        [MemoryPackOrder(1)]
        public int Count { get; set; }

        /// <summary>
        /// Rate.
        /// </summary>
        [MemoryPackOrder(2)]
        public int Rate { get; set; }
    }
}
